namespace Shared.Abstractions.Factories;

public enum DatabaseType
{
    DefaultConnection
}

public interface IDbConnectionFactory
{
    string GetConnectionString(DatabaseType databaseType);
    string GetDefault();
}