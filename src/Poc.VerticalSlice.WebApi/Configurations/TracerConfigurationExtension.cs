using Jaeger;
using Jaeger.Metrics;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using OpenTracing;
using OpenTracing.Util;

namespace Poc.VerticalSlice.WebApi.Configurations
{
    public static class TracerConfigurationExtension
    {
        public static IServiceCollection AddTracerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITracer>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();

                var jaegerHost = configuration["JAEGER_HOST"] ?? "localhost";
                var jaegerPort = int.Parse(configuration["JAEGER_PORT"] ?? "6831");

                var seenderConfig = new Configuration.SenderConfiguration(loggerFactory)
                    .WithAgentHost(jaegerHost)
                    .WithAgentPort(jaegerPort);

                var reporterConfig = new Configuration.ReporterConfiguration(loggerFactory)
                    .WithSender(seenderConfig);

                var metrics = new MetricsImpl(NoopMetricsFactory.Instance);

                var tracer = new Tracer.Builder("vertical-slice-webapi")
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(new ConstSampler(sample: true))
                    .WithReporter(reporterConfig.GetReporter(metrics))
                    .Build();

                GlobalTracer.Register(tracer);
                return tracer;
            });

            services.AddOpenTracingCoreServices(otBuilder =>
            {
                otBuilder.AddLoggerProvider();
                otBuilder.AddGenericDiagnostics();
                otBuilder.AddMicrosoftSqlClient();

                otBuilder.AddHttpHandler().ConfigureHttpHandler(options =>
                {
                    options.OperationNameResolver = (httpRequestMessage) =>
                    {
                        return $"{httpRequestMessage.Method} {httpRequestMessage.RequestUri?.AbsolutePath}";
                    };
                });

                otBuilder.AddAspNetCore().ConfigureAspNetCore(options =>
                {
                    options.LogEvents = false;

                    options.Hosting.OperationNameResolver = (httpContext) =>
                    {
                        return $"{httpContext.Request.Method} {httpContext.Request.Path}";
                    };
                });
            });

            return services;
        }
    }
}
