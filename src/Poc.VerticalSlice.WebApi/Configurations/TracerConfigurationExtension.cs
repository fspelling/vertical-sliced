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

                var jaegerHost = configuration["Tracing:JAEGER_HOST"] ?? "localhost";
                var jaegerPort = int.Parse(configuration["Tracing:JAEGER_PORT"] ?? "6831");
                var serviceName = configuration["Tracing:JAEGER_SERVICE_NAME"] ?? "vertical-slice-webapi";

                var seenderConfig = new Configuration.SenderConfiguration(loggerFactory)
                    .WithAgentHost(jaegerHost)
                    .WithAgentPort(jaegerPort);

                var reporterConfig = new Configuration.ReporterConfiguration(loggerFactory)
                    .WithSender(seenderConfig);

                var metrics = new MetricsImpl(NoopMetricsFactory.Instance);

                var tracer = new Tracer.Builder(serviceName)
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

        public static void UseTracerConfiguration(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var tracer = context.RequestServices.GetRequiredService<ITracer>();
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("TraceScope");

                var jaegerTraceId = tracer.ActiveSpan?.Context.TraceId ?? "";

                using (logger.BeginScope("JaegerTraceId:{JaegerTraceId}", jaegerTraceId))
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erro genérico ao processar a requisição");

                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            await context.Response.WriteAsJsonAsync(new { erro = "Erro interno ao processar a requisição." });
                        }
                    }
                }
            });
        }
    }
}
