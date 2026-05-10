namespace FuelGuard.Infrastructure.Ai;

/// <summary>
/// Hugging Face Inference API settings (configure via appsettings, user secrets, or environment variables).
/// </summary>
public sealed class AiOptions
{
    public const string SectionName = "AI";

    /// <summary>Logical provider label for logging and future switches.</summary>
    public string Provider { get; set; } = "HuggingFace";

    /// <summary>Hugging Face model id, e.g. Qwen/Qwen2.5-7B-Instruct.</summary>
    public string Model { get; set; } = "Qwen/Qwen2.5-7B-Instruct";

    /// <summary>Legacy inference host; override if Hugging Face routes you to a different endpoint.</summary>
    public string InferenceBaseUrl { get; set; } = "https://api-inference.huggingface.co";

    /// <summary>HF API token (prefer user secrets: AI:ApiKey).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>HTTP timeout for a single inference call.</summary>
    public int RequestTimeoutSeconds { get; set; } = 90;
}
