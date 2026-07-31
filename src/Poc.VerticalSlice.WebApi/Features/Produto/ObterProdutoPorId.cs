using Carter;
using FluentResults;
using FluentValidation;
using MediatR;
using Poc.VerticalSlice.WebApi.DataBase;
using Poc.VerticalSlice.WebApi.Shared.Extensions;

namespace Poc.VerticalSlice.WebApi.Features.Produto
{
    public static class ObterProdutoPorId
    {
        public sealed record Query(Guid Id) : IRequest<Result<Entities.Produto>>;

        public sealed class Validator : AbstractValidator<Query>
        {
            public Validator() => RuleFor(r => r.Id).NotEmpty();
        }

        public sealed class Handler(Repository repository, IValidator<Query> validator) : IRequestHandler<Query, Result<Entities.Produto>>
        {
            private readonly Repository _repository = repository;
            private readonly IValidator<Query> _validator = validator;

            public async Task<Result<Entities.Produto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var validateResult = _validator.Validate(request);

                if (!validateResult.IsValid)
                    return Result.Fail(validateResult.Errors.FirstOrDefault()!.ErrorMessage);

                var produto = await _repository.ObterPorId(request.Id);

                if (produto is null)
                    return Result.Fail("Produto nao encontrado.");

                return Result.Ok(produto);
            }
        }

        public sealed class Repository(VsaDbContext vsaDbContext)
        {
            private readonly VsaDbContext _vsaDbContext = vsaDbContext;

            public async Task<Entities.Produto?> ObterPorId(Guid id)
                => await _vsaDbContext.Produtos.FindAsync(id);
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
                .Produces<Result<Entities.Produto>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithOpenApi();
        }
    }
}
