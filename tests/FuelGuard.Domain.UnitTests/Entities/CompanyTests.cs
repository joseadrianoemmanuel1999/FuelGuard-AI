using FluentAssertions;
using FuelGuard.Domain.Entities;
using FuelGuard.Domain.Enums;

namespace FuelGuard.Domain.UnitTests.Entities;

public sealed class CompanyTests
{
    [Fact]
    public void NewCompany_HasEmptyCollections()
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Acme Fuel",
            Type = CompanyType.Distributor
        };

        company.Tanks.Should().BeEmpty();
        company.Transactions.Should().BeEmpty();
    }
}
