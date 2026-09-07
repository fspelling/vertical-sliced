using Poc.VerticalSlice.Application.Shared.ServiceExternal;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class ExternalApisConfig
    {
        public static IServiceCollection AddExternalApisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrlPessoaDocumento = configuration["Providers:pessoaDocumentoBaseUrl"] ?? "http://localhost:5002";
            var baseUrlPessoaNome = configuration["Providers:pessoaNomeBaseUrl"] ?? "http://localhost:5001";

            services.AddDependencyInjectionServiceExternal(baseUrlPessoaDocumento, baseUrlPessoaNome);
            return services;
        }
    }
}
