using FluentValidation;
using Poc.VerticalSlice.Application;
using Poc.VerticalSlice.Application.Features.Produto;

namespace Poc.VerticalSlice.WebApi.Config;

public static class DependencyInjectionConfigurationExtension
{
    public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

        services.AddScoped<CriarProduto.Repository>();
        services.AddScoped<ObterProdutoPorId.Repository>();

        return services;
    }
}
