using Carter;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class CarterConfigurationExtension
    {
        public static IServiceCollection AddCarterConfiguration(this IServiceCollection services)
        {
            services.AddCarter();
            return services;
        }

        public static void UseCarterConfiguration(this WebApplication app)
        {
            app.MapCarter();
        }
    }
}
