using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Poc.VerticalSlice.Application.Shared.ServiceExternal;

namespace Poc.VerticalSlice.Application.Features.Pessoa;

public static class GerarPessoa
{
    public sealed record Query : IRequest<Result<Shared.Entities.Pessoa>>;

    public sealed class Handler(ILogger<Handler> logger, IGeradorPessoaDocumentoAPI geradorPessoaDocumentoAPI, IGeradorPessoaNomeAPI geradorPessoaNomeAPI) : IRequestHandler<Query, Result<Shared.Entities.Pessoa>>
    {
        private readonly ILogger<Handler> _logger = logger;
        private readonly IGeradorPessoaDocumentoAPI _geradorPessoaDocumentoAPI = geradorPessoaDocumentoAPI;
        private readonly IGeradorPessoaNomeAPI _geradorPessoaNomeAPI = geradorPessoaNomeAPI;

        public async Task<Result<Shared.Entities.Pessoa>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Gerando uma nova pessoa de forma randomica.");

            var responsePessoaDocumento = await _geradorPessoaDocumentoAPI.GerarPessoaDocumento();
            var responsePessoaNome = await _geradorPessoaNomeAPI.GerarPessoaNome();

            if (responsePessoaDocumento is null || responsePessoaNome is null)
            {
                _logger.LogError("Erro ao gerar uma nova pessoa de forma randomica.");
                return Result.Fail<Shared.Entities.Pessoa>("Erro ao gerar uma nova pessoa de forma randomica.");
            }

            var pessoa = new Shared.Entities.Pessoa(responsePessoaDocumento.Cpf, responsePessoaDocumento.RG, responsePessoaNome.Nome);
            _logger.LogInformation("Nova pessoa gerada com sucesso: {@Pessoa}", pessoa);

            return Result.Ok(pessoa);
        }
    }
}