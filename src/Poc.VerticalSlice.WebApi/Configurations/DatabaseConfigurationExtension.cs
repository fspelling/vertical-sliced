using Microsoft.EntityFrameworkCore;
using Poc.VerticalSlice.Application.Shared.DbContexts;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class DatabaseConfigurationExtension
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
        {
            services.AddDbContext<VsaDbContext>(options => options.UseInMemoryDatabase("VsaDB"));
            services.AddDistributedMemoryCache();

            return services;
        }
    }
}
