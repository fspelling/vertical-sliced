using FluentResults;
using FluentValidation;
using MediatR;
using Poc.VerticalSlice.Application.Shared.DbContexts;

namespace Poc.VerticalSlice.Application.Features.Produto;

public static class ObterProdutoPorId
{
    public sealed record Query(Guid Id) : IRequest<Result<Shared.Entities.Produto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(r => r.Id).NotEmpty();
    }

    public sealed class Handler(Repository repository, IValidator<Query> validator) : IRequestHandler<Query, Result<Shared.Entities.Produto>>
    {
        private readonly Repository _repository = repository;
        private readonly IValidator<Query> _validator = validator;

        public async Task<Result<Shared.Entities.Produto>> Handle(Query request, CancellationToken cancellationToken)
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

        public async Task<Shared.Entities.Produto?> ObterPorId(Guid id)
            => await _vsaDbContext.Produtos.FindAsync(id);
    }
}