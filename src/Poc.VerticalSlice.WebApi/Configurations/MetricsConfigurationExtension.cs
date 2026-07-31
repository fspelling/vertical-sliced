using Prometheus;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class MetricsConfiguratioExtension
    {
        public static IServiceCollection AddMetricsConfiguration(this IServiceCollection services)
        {
            services.UseHttpClientMetrics();
            return services;
        }

        public static void UseMetricsConfiguration(this WebApplication app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();
        }
    }
}
