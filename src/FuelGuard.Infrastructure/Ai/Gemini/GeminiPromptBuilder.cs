namespace FuelGuard.Infrastructure.Ai.Gemini;

public static class GeminiPromptBuilder
{
    private const string SystemRole =
        """
        You are FuelGuard Operational Intelligence — an enterprise analyst for fuel integrity, loss prevention, and fraud investigations.
        Rules:
        - Use ONLY the OPERATIONAL FACTS provided. Never invent stations, volumes, IDs, or events.
        - Risk scores and anomaly severities in FACTS are authoritative; explain them, do not recalculate or contradict them.
        - Focus on fuel operations: variance, inventory mismatch, suspicious flow, smuggling indicators, compliance actions.
        - Be concise, actionable, and investigator-facing (not casual chatbot tone).
        - Output MUST be valid JSON only (no markdown fences, no prose outside JSON).
        """;

    public static string BuildChatPrompt(
        string question,
        string factsBlock,
        string? stationHint,
        string? periodHint,
        string? additionalContext)
    {
        var user = new System.Text.StringBuilder();
        user.AppendLine(SystemRole);
        user.AppendLine();
        user.AppendLine(factsBlock);
        user.AppendLine();
        if (!string.IsNullOrWhiteSpace(stationHint))
            user.AppendLine($"USER STATION HINT: {stationHint.Trim()}");
        if (!string.IsNullOrWhiteSpace(periodHint))
            user.AppendLine($"USER PERIOD HINT: {periodHint.Trim()}");
        if (!string.IsNullOrWhiteSpace(additionalContext))
            user.AppendLine($"ADDITIONAL CONTEXT: {additionalContext.Trim()}");
        user.AppendLine();
        user.AppendLine($"USER QUESTION: {question.Trim()}");
        user.AppendLine();
        user.AppendLine(
            """
            Respond with a single JSON object with exactly these keys:
            "answer" (string, markdown allowed inside the string for lists/bold),
            "riskLevel" (string: LOW|MEDIUM|HIGH|CRITICAL|UNKNOWN),
            "recommendations" (array of 3-6 short imperative strings),
            "explainability" (string, 2-4 sentences citing specific facts by station/severity/score)
            """);

        return user.ToString();
    }

    public static string BuildInsightsPrompt(string factsBlock) =>
        SystemRole + "\n\n" + factsBlock + "\n\n" +
        """
        TASK: Produce an executive operational intelligence brief as JSON only with keys:
        "operationalInsights" (array of 3-5 strings),
        "anomalyHighlights" (array of 2-4 strings referencing real anomalies from facts, or note if none),
        "recommendations" (array of 3-5 imperative actions),
        "executiveSummary" (string, max 5 sentences for leadership),
        "riskObservations" (array of 2-4 strings about risk posture from facts)
        """;
}
