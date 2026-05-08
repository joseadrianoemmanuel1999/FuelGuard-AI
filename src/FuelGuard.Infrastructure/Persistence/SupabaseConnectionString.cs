using Npgsql;

namespace FuelGuard.Infrastructure.Persistence;

public static class SupabaseConnectionString
{
    public static string ForEntityFramework(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }
}
