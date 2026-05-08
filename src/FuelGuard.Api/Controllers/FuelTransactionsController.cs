using FuelGuard.Api.Contracts;
using FuelGuard.Application.FuelTransactions;
using FuelGuard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FuelGuard.Api.Controllers;

[ApiController]
[Route("api/fuel-transactions")]
public sealed class FuelTransactionsController(CreateFuelTransactionHandler handler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateFuelTransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateFuelTransactionResponse>> CreateAsync(
        [FromBody] CreateFuelTransactionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateFuelTransactionCommand(
                request.CompanyId,
                request.TankId,
                request.QuantityLiters,
                request.MovementType,
                request.OccurredAt ?? DateTimeOffset.UtcNow);

            var result = await handler.HandleAsync(command, cancellationToken);

            return Created($"/api/fuel-transactions/{result.TransactionId}", new CreateFuelTransactionResponse(result.TransactionId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("demo/spike")]
    public Task<ActionResult<CreateFuelTransactionResponse>> DemoSpikeAsync(CancellationToken cancellationToken)
    {
        var stationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var dieselTankId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var request = new CreateFuelTransactionRequest
        {
            CompanyId = stationId,
            TankId = dieselTankId,
            QuantityLiters = 18_000,
            MovementType = FuelMovementType.Outbound,
            OccurredAt = DateTimeOffset.UtcNow
        };

        return CreateAsync(request, cancellationToken);
    }
}

public sealed record CreateFuelTransactionResponse(Guid TransactionId);

