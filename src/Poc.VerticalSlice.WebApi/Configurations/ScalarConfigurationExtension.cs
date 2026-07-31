using Prometheus;
using Scalar.AspNetCore;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class ScalarConfigurationExtension
    {
        public static void UseScalarConfiguration(this WebApplication app)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}
