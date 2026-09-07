using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal
{
    public static class Module
    {
        public static IServiceCollection AddDependencyInjectionServiceExternal(this IServiceCollection services, string baseUrlPessoaDocumento, string baseUrlPessoaNome)
        {
            services.AddRefitClient<IGeradorPessoaDocumentoAPI>().ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrlPessoaDocumento));
            services.AddRefitClient<IGeradorPessoaNomeAPI>().ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrlPessoaNome));

            return services;
        }
    }
}
