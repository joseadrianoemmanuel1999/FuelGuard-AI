using FluentAssertions;
using FuelGuard.Application.FuelTransactions;
using FuelGuard.Application.UnitTests.Support;
using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Enums;
using FuelGuard.Shared.Abstractions;
using FuelGuard.Shared.Events;
using Moq;

namespace FuelGuard.Application.UnitTests.FuelTransactions;

public sealed class CreateFuelTransactionHandlerTests
{
    private readonly Mock<IEventBus> _eventBus = new();

    [Fact]
    public async Task HandleAsync_WhenCompanyMissing_Throws()
    {
        await using var db = TestDb.Create();
        var handler = new CreateFuelTransactionHandler(db, _eventBus.Object);

        var command = new CreateFuelTransactionCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            FuelMovementType.Inbound,
            DateTimeOffset.UtcNow);

        var act = () => handler.HandleAsync(command);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Company*");
    }

    [Fact]
    public async Task HandleAsync_WhenQuantityNotPositive_Throws()
    {
        var companyId = Guid.NewGuid();
        var tankId = Guid.NewGuid();

        await using var db = TestDb.Create();
        db.Companies.Add(new Company { Id = companyId, Name = "Test Co", Type = CompanyType.GasStation });
        db.FuelTanks.Add(new FuelTank { Id = tankId, CompanyId = companyId, CapacityLiters = 10_000m, FuelType = "Diesel" });
        await db.SaveChangesAsync();

        var handler = new CreateFuelTransactionHandler(db, _eventBus.Object);
        var command = new CreateFuelTransactionCommand(
            companyId,
            tankId,
            0m,
            FuelMovementType.Inbound,
            DateTimeOffset.UtcNow);

        var act = () => handler.HandleAsync(command);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*greater than zero*");
    }

    [Fact]
    public async Task HandleAsync_WhenValid_PersistsAndPublishesEvent()
    {
        var companyId = Guid.NewGuid();
        var tankId = Guid.NewGuid();

        await using var db = TestDb.Create();
        db.Companies.Add(new Company { Id = companyId, Name = "Test Co", Type = CompanyType.GasStation });
        db.FuelTanks.Add(new FuelTank { Id = tankId, CompanyId = companyId, CapacityLiters = 10_000m, FuelType = "Diesel" });
        await db.SaveChangesAsync();

        FuelTransactionCreatedEvent? published = null;
        _eventBus
            .Setup(b => b.PublishAsync(It.IsAny<FuelTransactionCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<FuelTransactionCreatedEvent, CancellationToken>((e, _) => published = e)
            .Returns(Task.CompletedTask);

        var handler = new CreateFuelTransactionHandler(db, _eventBus.Object);
        var occurredAt = DateTimeOffset.Parse("2026-05-01T12:00:00Z");

        var result = await handler.HandleAsync(new CreateFuelTransactionCommand(
            companyId,
            tankId,
            250.5m,
            FuelMovementType.Inbound,
            occurredAt));

        result.TransactionId.Should().NotBeEmpty();
        db.FuelTransactions.Should().ContainSingle(t =>
            t.Id == result.TransactionId &&
            t.QuantityLiters == 250.5m &&
            t.MovementType == FuelMovementType.Inbound);

        published.Should().NotBeNull();
        published!.TransactionId.Should().Be(result.TransactionId);
        published.CompanyId.Should().Be(companyId);
        published.TankId.Should().Be(tankId);
        _eventBus.Verify(
            b => b.PublishAsync(It.IsAny<FuelTransactionCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
