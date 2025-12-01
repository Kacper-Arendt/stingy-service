using Microsoft.Extensions.Configuration;
using Shared.Abstractions.Factories;

namespace Shared.Infrastructure.Factories;

public class DbConnectionStringFactory: IDbConnectionStringFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionStringFactory(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public string GetDefault()
    {
        var connectionString = _configuration.GetConnectionString(nameof(DatabaseType.DefaultConnection));

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"No connection string found for database type '{DatabaseType.DefaultConnection}'");
        }

        return connectionString;
    }

    public string GetConnectionString(DatabaseType databaseType)
    {
        var connectionString = _configuration.GetConnectionString(databaseType.ToString());

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"No connection string found for database type: {databaseType}");
        }

        return connectionString;
    }
}