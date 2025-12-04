using FluentMigrator.Runner;
using Azure.Identity;

var tempConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var keyVaultUrl = tempConfig["KeyVault"];

if (string.IsNullOrWhiteSpace(keyVaultUrl))
    throw new InvalidOperationException("KeyVault:Url is missing from configuration.");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential())
    .Build();

var connectionString = configuration["ConnectionStrings:DefaultConnection"];

var serviceProvider = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();