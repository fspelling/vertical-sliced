using Prometheus;

namespace Poc.VerticalSlice.WebApi.Endpoints.Filters
{
    public sealed class MetricsFilter<T>(T metric) : IEndpointFilter
    {
        private readonly T _metric = metric;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            return _metric switch
            {
                Counter metric => await ExecuteMetricCounter(context, next, metric),
                Gauge metric => await ExecuteMetricGauge(context, next, metric),
                Histogram metric => await ExecuteMetricHistogram(context, next, metric),
                _ => await next(context)
            };
        }

        private async ValueTask<object?> ExecuteMetricCounter(EndpointFilterInvocationContext context, EndpointFilterDelegate next, Counter metric)
        {
            object? result = await next(context);

            if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 })
                metric.WithLabels("sucess").Inc();
            else
                metric.WithLabels("error").Inc();

            return result;
        }

        private async ValueTask<object?> ExecuteMetricGauge(EndpointFilterInvocationContext context, EndpointFilterDelegate next, Gauge metric)
        {
            metric.Inc();
            return await next(context);
        }

        private async ValueTask<object?> ExecuteMetricHistogram(EndpointFilterInvocationContext context, EndpointFilterDelegate next, Histogram metric)
        {
            using (metric.NewTimer())
                return await next(context);
        }
    }
}
