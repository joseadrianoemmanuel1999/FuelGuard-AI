namespace FuelGuard.Application.Abstractions.Gemini;

/// <summary>
/// Interactive Gemini layer for operational Q&amp;A and insights. Never throws to API callers.
/// </summary>
public interface IGeminiService
{
    Task<GeminiChatResponse> ChatAsync(GeminiChatRequest request, CancellationToken cancellationToken = default);

    Task<GeminiInsightsResponse> GetInsightsAsync(CancellationToken cancellationToken = default);
}
