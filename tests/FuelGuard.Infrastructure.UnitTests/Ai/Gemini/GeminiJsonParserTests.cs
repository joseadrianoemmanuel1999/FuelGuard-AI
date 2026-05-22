using FluentAssertions;
using FuelGuard.Infrastructure.Ai.Gemini;

namespace FuelGuard.Infrastructure.UnitTests.Ai.Gemini;

public sealed class GeminiJsonParserTests
{
    [Fact]
    public void ParseChat_ExtractsJsonFromMarkdownFence()
    {
        const string raw = """
            Here is the response:
            ```json
            {"answer":"Tank level mismatch","riskLevel":"HIGH","recommendations":["Reconcile dip"],"explainability":"Variance vs baseline"}
            ```
            """;

        var dto = GeminiJsonParser.ParseChat(raw);

        dto.Should().NotBeNull();
        dto!.Answer.Should().Be("Tank level mismatch");
        dto.RiskLevel.Should().Be("HIGH");
        dto.Recommendations.Should().ContainSingle("Reconcile dip");
    }

    [Fact]
    public void ParseInsights_WhenInvalidJson_ReturnsNull()
    {
        GeminiJsonParser.ParseInsights("not json at all").Should().BeNull();
    }

    [Fact]
    public void ParseInsights_WhenValid_ReturnsDto()
    {
        const string raw = """{"executiveSummary":"Stable week","operationalInsights":["No anomalies"],"anomalyHighlights":[],"recommendations":["Continue monitoring"],"riskObservations":[]}""";

        var dto = GeminiJsonParser.ParseInsights(raw);

        dto.Should().NotBeNull();
        dto!.ExecutiveSummary.Should().Be("Stable week");
        dto.OperationalInsights.Should().ContainSingle("No anomalies");
    }
}
