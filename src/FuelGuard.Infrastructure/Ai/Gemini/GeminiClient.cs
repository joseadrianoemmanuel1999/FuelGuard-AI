using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuelGuard.Infrastructure.Ai.Gemini;

/// <summary>
/// Minimal HTTP client for Google Generative Language API (Gemini Flash).
/// </summary>
public sealed class GeminiClient
{
    public const string HttpClientName = "FuelGuard.Gemini";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<GeminiOptions> _options;
    private readonly ILogger<GeminiClient> _logger;

    public GeminiClient(
        IHttpClientFactory httpClientFactory,
        IOptions<GeminiOptions> options,
        ILogger<GeminiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.Value.GeminiApiKey);

    public async Task<string?> GenerateJsonAsync(string prompt, CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (string.IsNullOrWhiteSpace(opts.GeminiApiKey))
            return null;

        var model = string.IsNullOrWhiteSpace(opts.GeminiModel)
            ? "gemini-2.0-flash"
            : opts.GeminiModel.Trim();

        var maxRetries = Math.Clamp(opts.GeminiMaxRetries, 0, 5);
        var sw = Stopwatch.StartNew();

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(HttpClientName);
                var url =
                    $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(opts.GeminiApiKey.Trim())}";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[] { new { text = prompt } }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.22,
                        maxOutputTokens = 2048,
                        responseMimeType = "application/json"
                    }
                };

                using var response = await client.PostAsJsonAsync(url, payload, SerializerOptions, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var retryable = (int)response.StatusCode is 429 or 500 or 502 or 503;
                    _logger.LogWarning(
                        "Gemini request failed ({Status}) attempt {Attempt} in {Ms}ms: {Body}",
                        (int)response.StatusCode,
                        attempt + 1,
                        sw.ElapsedMilliseconds,
                        Truncate(body, 400));

                    if (retryable && attempt < maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(400 * (attempt + 1)), cancellationToken);
                        continue;
                    }

                    return null;
                }

                var text = ExtractText(body);
                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("Gemini returned empty text in {Ms}ms.", sw.ElapsedMilliseconds);
                    return null;
                }

                _logger.LogInformation("Gemini inference OK in {Ms}ms.", sw.ElapsedMilliseconds);
                return text;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Gemini request timed out after {Ms}ms.", sw.ElapsedMilliseconds);
                return null;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                _logger.LogWarning(ex, "Gemini request threw on attempt {Attempt}.", attempt + 1);
                await Task.Delay(TimeSpan.FromMilliseconds(400 * (attempt + 1)), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini request failed after {Ms}ms.", sw.ElapsedMilliseconds);
                return null;
            }
        }

        return null;
    }

    private static string? ExtractText(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                return null;

            var content = candidates[0].GetProperty("content");
            if (!content.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
                return null;

            return parts[0].TryGetProperty("text", out var text) ? text.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max] + "…";
}
