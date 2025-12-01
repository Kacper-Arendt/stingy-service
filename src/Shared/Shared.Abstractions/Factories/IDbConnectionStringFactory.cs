namespace Shared.Abstractions.Factories;

public enum DatabaseType
{
    DefaultConnection
}

public interface IDbConnectionStringFactory
{
    string GetConnectionString(DatabaseType databaseType);
    string GetDefault();
}