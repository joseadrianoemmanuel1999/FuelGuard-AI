using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FuelGuard.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuelGuard.Infrastructure.Ai;

/// <summary>
/// Calls the Hugging Face Inference API for open-source models (default: Qwen2.5-7B-Instruct).
/// Never throws to callers — failures return null so deterministic scoring always completes.
/// </summary>
public sealed class HuggingFaceAiService : IAiService
{
    public const string HttpClientName = "FuelGuard.HuggingFace";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AiOptions> _options;
    private readonly ILogger<HuggingFaceAiService> _logger;

    public HuggingFaceAiService(
        IHttpClientFactory httpClientFactory,
        IOptions<AiOptions> options,
        ILogger<HuggingFaceAiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<AIExplanationResponse?> ExplainRiskAsync(
        RiskAssessmentContext context,
        CancellationToken cancellationToken = default)
    {
        var opts = _options.Value;
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
        {
            _logger.LogDebug("Hugging Face AI disabled (empty AI:ApiKey).");
            return null;
        }

        var sw = Stopwatch.StartNew();
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientName);
            var modelPath = opts.Model.Trim().Trim('/');
            var url = $"{opts.InferenceBaseUrl.TrimEnd('/')}/models/{modelPath}";

            var inputs = RiskFraudAnalysisPromptBuilder.BuildInferenceInputs(context);
            var payload = new Dictionary<string, object?>
            {
                ["inputs"] = inputs,
                ["parameters"] = new Dictionary<string, object?>
                {
                    ["max_new_tokens"] = 512,
                    ["temperature"] = 0.18,
                    ["return_full_text"] = false
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey.Trim());

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Hugging Face inference failed ({StatusCode}) in {ElapsedMs} ms: {Body}",
                    (int)response.StatusCode,
                    sw.ElapsedMilliseconds,
                    Truncate(body, 500));
                return null;
            }

            var generated = ExtractGeneratedText(body);
            if (string.IsNullOrWhiteSpace(generated))
            {
                _logger.LogWarning("Hugging Face returned no generated_text in {ElapsedMs} ms.", sw.ElapsedMilliseconds);
                return null;
            }

            var parsed = TryParseExplanation(generated);
            if (parsed is null)
            {
                _logger.LogWarning(
                    "Could not parse AI JSON explanation. Raw (truncated): {Raw}",
                    Truncate(generated, 800));
            }
            else
            {
                _logger.LogInformation(
                    "Hugging Face inference succeeded in {ElapsedMs} ms (confidence {Confidence}).",
                    sw.ElapsedMilliseconds,
                    parsed.Confidence);
            }

            return parsed;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Hugging Face inference timed out after {ElapsedMs} ms.", sw.ElapsedMilliseconds);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Hugging Face inference threw after {ElapsedMs} ms.", sw.ElapsedMilliseconds);
            return null;
        }
    }

    private static string? ExtractGeneratedText(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var first = root[0];
                if (first.ValueKind == JsonValueKind.Object &&
                    first.TryGetProperty("generated_text", out var gt))
                    return gt.GetString();
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("generated_text", out var single))
                    return single.GetString();

                if (root.TryGetProperty("error", out _))
                    return null;
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static AIExplanationResponse? TryParseExplanation(string raw)
    {
        var slice = ExtractJsonObjectSlice(raw);
        if (slice is null)
            return null;

        try
        {
            var dto = JsonSerializer.Deserialize<AiExplanationJsonDto>(slice, SerializerOptions);
            if (dto is null || string.IsNullOrWhiteSpace(dto.Summary))
                return null;

            var actions = dto.RecommendedActions is { Count: > 0 }
                ? (IReadOnlyList<string>)dto.RecommendedActions.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()).ToList()
                : Array.Empty<string>();

            var confidence = string.IsNullOrWhiteSpace(dto.Confidence)
                ? "LOW"
                : dto.Confidence.Trim().ToUpperInvariant();

            if (confidence is not ("LOW" or "MEDIUM" or "HIGH"))
                confidence = "MEDIUM";

            return new AIExplanationResponse(dto.Summary.Trim(), actions, confidence);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ExtractJsonObjectSlice(string raw)
    {
        var trimmed = raw.Trim();
        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start < 0 || end <= start)
            return null;

        return trimmed[start..(end + 1)];
    }

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max)
            return value;

        return value[..max] + "…";
    }

    private sealed class AiExplanationJsonDto
    {
        public string? Summary { get; set; }
        public List<string>? RecommendedActions { get; set; }
        public string? Confidence { get; set; }
    }
}