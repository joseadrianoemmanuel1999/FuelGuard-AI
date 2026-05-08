using FuelGuard.Application.FuelTransactions;
using Microsoft.Extensions.DependencyInjection;

namespace FuelGuard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateFuelTransactionHandler>();
        return services;
    }
}
