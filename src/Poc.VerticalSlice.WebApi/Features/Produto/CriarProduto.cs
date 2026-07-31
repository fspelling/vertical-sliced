using Carter;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Poc.VerticalSlice.WebApi.Config;
using Poc.VerticalSlice.WebApi.DataBase;
using Poc.VerticalSlice.WebApi.EndpointFilters;
using Poc.VerticalSlice.WebApi.Shared.Extensions;
using Prometheus;

namespace Poc.VerticalSlice.WebApi.Features.Produto
{
    public static class CriarProduto
    {
        public sealed record Request(string Nome, string? Descricao, decimal Preco);

        public sealed record Command(string Nome, string? Descricao, decimal Preco) : IRequest<Result<Guid>>;

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(r => r.Nome).NotEmpty();
                RuleFor(r => r.Preco).NotEmpty();
            }
        }

        public sealed class Handler(Repository repository, IValidator<Command> validator) : IRequestHandler<Command, Result<Guid>>
        {
            private readonly Repository _repository = repository;
            private readonly IValidator<Command> _validator = validator;

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validateResult = _validator.Validate(request);

                if (!validateResult.IsValid)
                    return Result.Fail(validateResult.Errors.FirstOrDefault()!.ErrorMessage);

                var produto = new Entities.Produto(request.Nome, request.Descricao, request.Preco);

                await _repository.Criar(produto);
                return Result.Ok(produto.Id);
            }
        }

        public sealed class Repository(VsaDbContext vsaDbContext)
        {
            private readonly VsaDbContext _vsaDbContext = vsaDbContext;

            public async Task Criar(Entities.Produto produto)
                => await _vsaDbContext.AddAsync(produto);
        }
    }

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
}
