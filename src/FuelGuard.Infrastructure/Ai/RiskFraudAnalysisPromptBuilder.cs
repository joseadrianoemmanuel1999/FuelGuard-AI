using System.Text;
using FuelGuard.Application.Abstractions;

namespace FuelGuard.Infrastructure.Ai;

/// <summary>
/// Builds a strict, anti-hallucination prompt for open-source instruct models (Qwen2.5 family).
/// Output contract: a single JSON object only — parsed by <see cref="HuggingFaceAiService"/>.
/// </summary>
public static class RiskFraudAnalysisPromptBuilder
{
    private const string SystemBlock =
        """
        You are a senior fuel-integrity and fraud-investigations analyst assisting human investigators.
        Rules you MUST follow:
        - Use ONLY the structured FACTS provided in the user message. Do not invent companies, volumes, dates, or events.
        - The final numeric risk score and risk level are ALREADY decided by deterministic systems; you must NOT change them and must NOT output a different score.
        - Your job is to explain, in operational language, why the situation warrants scrutiny and what investigators should do next.
        - Be concise: no markdown, no code fences, no prose outside JSON.
        - Output MUST be a single JSON object with exactly these keys:
          "summary" (string, max 4 sentences),
          "recommendedActions" (array of 3 to 6 short imperative strings),
          "confidence" (string enum: "LOW", "MEDIUM", or "HIGH") reflecting how well the facts support your narrative given their sparsity.
        - If facts are insufficient to justify strong claims, lower confidence and keep recommendations generic (e.g. validate telemetry, reconcile dip readings).
        """;

    /// <summary>
    /// Single-string prompt for Hugging Face text-generation inference (works with Qwen2.5-Instruct and similar).
    /// </summary>
    public static string BuildInferenceInputs(RiskAssessmentContext context)
    {
        var facts = new StringBuilder();
        facts.AppendLine("FACTS (authoritative — do not contradict):");
        facts.AppendLine($"- companyId: {context.CompanyId}");
        if (context.AnomalyId is { } aid)
            facts.AppendLine($"- anomalyId: {aid}");
        if (context.TankId is { } tid)
            facts.AppendLine($"- tankId: {tid}");
        facts.AppendLine($"- severityLabel: {context.Severity}");
        facts.AppendLine($"- expectedValue (model baseline / reference): {context.ExpectedValue}");
        facts.AppendLine($"- actualValue (observed signal): {context.ActualValue}");
        facts.AppendLine($"- previousDeterministicScore: {context.PreviousScore}");
        facts.AppendLine($"- newDeterministicScore (final — do not alter): {context.NewScore}");
        facts.AppendLine($"- currentRiskLevel (final — do not alter): {context.CurrentRiskLevel}");
        facts.AppendLine($"- anomalyExplanation: {context.AnomalyExplanation}");
        facts.AppendLine();
        facts.AppendLine("TASK: Write the JSON object described in the system rules. No other text.");

        var user = facts.ToString();

        return
            "SYSTEM INSTRUCTIONS:\n" + SystemBlock + "\n\nUSER MESSAGE:\n" + user + "\n\nASSISTANT (JSON only, no markdown):\n";
    }
}
