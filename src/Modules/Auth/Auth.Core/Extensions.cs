using Auth.Core.Database;
using Auth.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.SqlServer;

namespace Auth.Core;

public static class Extensions
{
    public static IServiceCollection AddAuthCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddSqlServerWithEfCore<UsersEfContext>(connectionString);

        return services;
    }    
}