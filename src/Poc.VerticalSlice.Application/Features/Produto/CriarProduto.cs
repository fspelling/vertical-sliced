using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Poc.VerticalSlice.Application.Shared.DbContexts;
using System.Text.Json;

namespace Poc.VerticalSlice.Application.Features.Produto;

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

    public sealed class Handler(ILogger<Handler> logger, Repository repository, IValidator<Command> validator) : IRequestHandler<Command, Result<Guid>>
    {
        private readonly Repository _repository = repository;
        private readonly IValidator<Command> _validator = validator;
        private readonly ILogger<Handler> _logger = logger;

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Criando produto. Request: {JsonSerializer.Serialize(request)}");
                var validateResult = _validator.Validate(request);

                if (!validateResult.IsValid)
                {
                    _logger.LogWarning($"Produto nao pode ser criado: {request}");
                    return Result.Fail(validateResult.Errors.FirstOrDefault()!.ErrorMessage);
                }

                var produto = new Shared.Entities.Produto(request.Nome, request.Descricao, request.Preco);
                await _repository.Criar(produto);

                _logger.LogInformation($"Produto criado com sucesso. Produto: {JsonSerializer.Serialize(produto)}");
                return Result.Ok(produto.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao criar produto: {JsonSerializer.Serialize(request)}");
                throw;
            }
        }
    }

    public sealed class Repository(VsaDbContext vsaDbContext)
    {
        private readonly VsaDbContext _vsaDbContext = vsaDbContext;

        public async Task Criar(Shared.Entities.Produto produto)
            => await _vsaDbContext.AddAsync(produto);
    }
}
