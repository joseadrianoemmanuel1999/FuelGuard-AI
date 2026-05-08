using FuelGuard.Application.Abstractions;
using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Enums;
using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Application.FuelTransactions;

public sealed record CreateFuelTransactionCommand(
    Guid CompanyId,
    Guid TankId,
    decimal QuantityLiters,
    FuelMovementType MovementType,
    DateTimeOffset OccurredAt);

public sealed record CreateFuelTransactionResult(Guid TransactionId);

public sealed class CreateFuelTransactionHandler(
    IApplicationDbContext db,
    IEventBus eventBus)
{
    public async Task<CreateFuelTransactionResult> HandleAsync(
        CreateFuelTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        var companyExists = await db.Companies.AnyAsync(c => c.Id == command.CompanyId, cancellationToken);
        if (!companyExists)
            throw new InvalidOperationException($"Company '{command.CompanyId}' was not found.");

        var tank = await db.FuelTanks.SingleOrDefaultAsync(
            t => t.Id == command.TankId && t.CompanyId == command.CompanyId,
            cancellationToken);
        if (tank is null)
            throw new InvalidOperationException(
                $"Tank '{command.TankId}' was not found for company '{command.CompanyId}'.");

        if (command.QuantityLiters <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        var transaction = new FuelTransaction
        {
            Id = Guid.NewGuid(),
            CompanyId = command.CompanyId,
            TankId = command.TankId,
            QuantityLiters = command.QuantityLiters,
            MovementType = command.MovementType,
            OccurredAt = command.OccurredAt
        };

        db.FuelTransactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);

        var correlationId = Guid.NewGuid();

        var domainEvent = new FuelTransactionCreatedEvent(
            transaction.Id,
            transaction.CompanyId,
            transaction.TankId,
            transaction.QuantityLiters,
            transaction.MovementType.ToString(),
            transaction.OccurredAt)
        {
            CorrelationId = correlationId
        };

        await eventBus.PublishAsync(domainEvent, cancellationToken);

        return new CreateFuelTransactionResult(transaction.Id);
    }
}

