using System.Text.Json;
using System.Text.Json.Serialization;

namespace FuelGuard.Infrastructure.Ai.Gemini;

internal static class GeminiJsonParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ChatJsonDto? ParseChat(string? raw) => Parse<ChatJsonDto>(raw);

    public static InsightsJsonDto? ParseInsights(string? raw) => Parse<InsightsJsonDto>(raw);

    private static T? Parse<T>(string? raw) where T : class
    {
        var slice = ExtractJsonObject(raw);
        if (slice is null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(slice, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ExtractJsonObject(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var trimmed = raw.Trim();
        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start < 0 || end <= start)
            return null;

        return trimmed[start..(end + 1)];
    }

    internal sealed class ChatJsonDto
    {
        public string? Answer { get; set; }
        public string? RiskLevel { get; set; }
        public List<string>? Recommendations { get; set; }
        public string? Explainability { get; set; }
    }

    internal sealed class InsightsJsonDto
    {
        public List<string>? OperationalInsights { get; set; }
        public List<string>? AnomalyHighlights { get; set; }
        public List<string>? Recommendations { get; set; }
        public string? ExecutiveSummary { get; set; }
        public List<string>? RiskObservations { get; set; }
    }
}
