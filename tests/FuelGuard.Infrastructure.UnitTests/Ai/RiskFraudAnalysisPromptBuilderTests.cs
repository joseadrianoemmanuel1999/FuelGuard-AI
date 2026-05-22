using FluentAssertions;
using FuelGuard.Application.Abstractions;
using FuelGuard.Infrastructure.Ai;
using FuelGuard.Shared.Models;

namespace FuelGuard.Infrastructure.UnitTests.Ai;

public sealed class RiskFraudAnalysisPromptBuilderTests
{
    [Fact]
    public void BuildInferenceInputs_IncludesAuthoritativeFacts()
    {
        var context = new RiskAssessmentContext(
            CompanyId: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            AnomalyId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            TankId: Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Severity: "HIGH",
            ExpectedValue: 1000m,
            ActualValue: 850m,
            PreviousScore: 40,
            NewScore: 72,
            CurrentRiskLevel: RiskLevel.High,
            AnomalyExplanation: "Unexpected outflow");

        var prompt = RiskFraudAnalysisPromptBuilder.BuildInferenceInputs(context);

        prompt.Should().Contain("companyId: 11111111-1111-1111-1111-111111111111");
        prompt.Should().Contain("anomalyId: 22222222-2222-2222-2222-222222222222");
        prompt.Should().Contain("tankId: 33333333-3333-3333-3333-333333333333");
        prompt.Should().Contain("newDeterministicScore (final — do not alter): 72");
        prompt.Should().Contain("currentRiskLevel (final — do not alter): High");
        prompt.Should().Contain("ASSISTANT (JSON only, no markdown):");
    }
}
