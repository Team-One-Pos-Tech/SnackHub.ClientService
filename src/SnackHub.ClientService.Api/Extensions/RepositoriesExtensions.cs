using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnackHub.ClientService.Api.Configuration;
using SnackHub.ClientService.Domain.Contracts;
using SnackHub.ClientService.Infra.Repositories;
using SnackHub.ClientService.Infra.Repositories.Context;

namespace SnackHub.ClientService.Api.Extensions;

public static class RepositoriesExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IClientRepository, ClientRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("PostgreSQL").Get<PostgreSQLSettings>();
        var connectionString = $"Host={settings.Host};Username={settings.UserName};Password={settings.Password};Database={settings.Database}";
        
        serviceCollection
            .AddDbContext<ClientDbContext>(options => 
                options.UseNpgsql(connectionString));

        return serviceCollection;
    }
}