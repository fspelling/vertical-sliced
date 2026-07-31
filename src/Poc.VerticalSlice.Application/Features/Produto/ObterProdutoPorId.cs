using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Poc.VerticalSlice.Application.Shared.DbContexts;
using System.Text.Json;

namespace Poc.VerticalSlice.Application.Features.Produto;

public static class ObterProdutoPorId
{
    public sealed record Query(Guid Id) : IRequest<Result<Shared.Entities.Produto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(r => r.Id).NotEmpty();
    }

    public sealed class Handler(ILogger<Handler> logger, Repository repository, IValidator<Query> validator) : IRequestHandler<Query, Result<Shared.Entities.Produto>>
    {
        private readonly Repository _repository = repository;
        private readonly IValidator<Query> _validator = validator;
        private readonly ILogger<Handler> _logger = logger;

        public async Task<Result<Shared.Entities.Produto>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Buscando produto: {request.Id}");
                var validateResult = _validator.Validate(request);

                if (!validateResult.IsValid)
                    return Result.Fail(validateResult.Errors.FirstOrDefault()!.ErrorMessage);

                var produto = await _repository.ObterPorId(request.Id);

                if (produto is null)
                {
                    _logger.LogWarning($"Produto nao encontrado: {request.Id}");
                    return Result.Fail("Produto nao encontrado.");
                }

                _logger.LogInformation($"Produto encontrado: {JsonSerializer.Serialize(produto)}");
                return Result.Ok(produto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao buscar produto: {request.Id}");
                throw;
            }
        }
    }

    public sealed class Repository(VsaDbContext vsaDbContext)
    {
        private readonly VsaDbContext _vsaDbContext = vsaDbContext;

        public async Task<Shared.Entities.Produto?> ObterPorId(Guid id)
            => await _vsaDbContext.Produtos.FindAsync(id);
    }
}