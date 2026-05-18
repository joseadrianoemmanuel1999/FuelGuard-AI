using FuelGuard.Application.Abstractions.Gemini;

namespace FuelGuard.Infrastructure.Ai.Gemini;

public sealed class GeminiService(GeminiChatService chatService, GeminiInsightsService insightsService) : IGeminiService
{
    public Task<GeminiChatResponse> ChatAsync(GeminiChatRequest request, CancellationToken cancellationToken) =>
        chatService.ChatAsync(request, cancellationToken);

    public Task<GeminiInsightsResponse> GetInsightsAsync(CancellationToken cancellationToken) =>
        insightsService.GetInsightsAsync(cancellationToken);
}
