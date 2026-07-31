using Prometheus;

namespace Poc.VerticalSlice.WebApi.Config
{
    public static class MetricsRegistry
    {
        public static readonly Counter ProdutosTotal = Metrics.CreateCounter("produtos_total",
                                                                             "Total de produtos processados", 
                                                                             configuration: new CounterConfiguration { LabelNames = [ "status" ] });

        public static readonly Gauge ProdutosProcessamento = Metrics.CreateGauge("produtos_processamento",
                                                                                 "Produtos sendo processados no momento");

        public static readonly Histogram ProdutosTempoProcessamento = Metrics.CreateHistogram("produtos_tempo_processamento",
                                                                                 "Tempod e processamento dos produtos");
    }
}
