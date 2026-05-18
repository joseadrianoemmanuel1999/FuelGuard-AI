namespace FuelGuard.Infrastructure.Ai.Gemini;

/// <summary>
/// Gemini settings under the shared <c>AI</c> configuration section.
/// </summary>
public sealed class GeminiOptions
{
    public const string SectionName = "AI";

    public string GeminiApiKey { get; set; } = string.Empty;

    public string GeminiModel { get; set; } = "gemini-2.0-flash";

    public int GeminiMaxRetries { get; set; } = 2;

    public int RequestTimeoutSeconds { get; set; } = 60;
}
