using Poc.VerticalSlice.Application.Features.Produto;
using System.Reflection;

namespace Poc.VerticalSlice.WebApi.Config
{
    public static class DependencyInjectionConfigurationExtension
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services, Assembly assembly)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));

            services.AddScoped<CriarProduto.Repository>();
            services.AddScoped<ObterProdutoPorId.Repository>();

            return services;
        }
    }
}
