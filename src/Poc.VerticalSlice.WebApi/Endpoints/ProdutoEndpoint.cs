using Carter;
using FluentResults;
using Mapster;
using MediatR;
using Poc.VerticalSlice.Application.Features.Produto;
using Poc.VerticalSlice.Application.Shared.ObservabilityRegistry;
using Poc.VerticalSlice.WebApi.Endpoints.Filters;
using Poc.VerticalSlice.WebApi.Extensions;
using Prometheus;

namespace Poc.VerticalSlice.WebApi.Endpoints
{
    public class CriarProdutoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var endpoint = app.MapPost("api/produtos", async (CriarProduto.Request request, ISender sender) =>
            {
                var mapRequest = request.Adapt<CriarProduto.Command>();
                var result = await sender.Send(mapRequest);

                return result!.ToResultCustom();
            });

            ConfigMetadata(endpoint);
        }

        private void ConfigMetadata(RouteHandlerBuilder builder)
        {
            builder
                .WithTags("Produtos")
                .WithDescription("Cria um novo produto a partir dos dados enviados no corpo da requisição.")
                .Accepts<CriarProduto.Request>("application/json")
                .Produces<Result<Guid>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithOpenApi()
                .AddEndpointFilter<IdempotencyFilter>()
                .AddEndpointFilter(new MetricsFilter<Counter>(MetricsRegistry.ProdutosTotal))
                .AddEndpointFilter(new MetricsFilter<Gauge>(MetricsRegistry.ProdutosProcessamento))
                .AddEndpointFilter(new MetricsFilter<Histogram>(MetricsRegistry.ProdutosTempoProcessamento));
        }
    }

    public class ObterProdutoPorIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var endpoint = app.MapGet("api/produtos/{id:Guid}", async (Guid id, ISender sender) =>
            {
                var query = new ObterProdutoPorId.Query(id);
                var result = await sender.Send(query);

                return result.ToResultCustom();
            });

            ConfigMetadata(endpoint);
        }

        private void ConfigMetadata(RouteHandlerBuilder builder)
        {
            builder
                .WithTags("Produtos")
                .WithDescription("Obter um produto existente a partir do id solicitado.")
                .Accepts<Guid>("application/json")
                .Produces<Result<Application.Shared.Entities.Produto>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithOpenApi();
        }
    }
}
