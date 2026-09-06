using FluentResults;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Poc.VerticalSlice.Application.Shared.Dtos.ExternalServices;
using Refit;

namespace Poc.VerticalSlice.Application.Features.Pessoa;

public static class GerarPessoa
{
    public sealed record Query : IRequest<Result<Shared.Entities.Pessoa>>;

    public sealed class Handler(ILogger<Handler> logger) : IRequestHandler<Query, Result<Shared.Entities.Pessoa>>
    {
        private readonly ILogger<Handler> _logger = logger;

        public async Task<Result<Shared.Entities.Pessoa>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Gerando uma nova pessoa de forma randomica.");

            var responsePessoaDocumento = await new GeradorPessoaDocumentoAPI().GerarPessoaDocumento();
            var responsePessoaNome = await new GeradorPessoaNomeAPI().GerarPessoaNome();

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

    // TODO: Refatorar RefitBase
    // TODO: Utilizar interface como injecao para IGeradorPessoaAPI
    // TODO: Parametrizar a URL base do serviço de geração de pessoas, para que possamos alterar facilmente o endpoint sem precisar modificar o código-fonte.

    [Headers("accept: */*")]
    public interface IGeradorPessoaDocumentoAPI
    {
        [Get("/documento")]
        Task<PessoaDocumentoResponse> GerarPessoaDocumento();
    }

    [Headers("accept: */*")]
    public interface IGeradorPessoaNomeAPI
    {
        [Get("/nomecompleto")]
        Task<PessoaNomeResponse> GerarPessoaNome();
    }

    public abstract class RefitBase(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        protected static T Create<T>(string baseUrl)
        {
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                })
            };
            return RestService.For<T>(baseUrl, settings);
        }
    }

    public sealed class GeradorPessoaDocumentoAPI
    {
        public static IGeradorPessoaDocumentoAPI Create(string baseUrl)
        {
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                })
            };
            return RestService.For<IGeradorPessoaDocumentoAPI>(baseUrl, settings);
        }

        public async Task<PessoaDocumentoResponse> GerarPessoaDocumento()
        {
            var api = Create("http://localhost:5002");
            return await api.GerarPessoaDocumento();
        }
    }

    public sealed class GeradorPessoaNomeAPI
    {
        public static IGeradorPessoaNomeAPI Create(string baseUrl)
        {
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                })
            };
            return RestService.For<IGeradorPessoaNomeAPI>(baseUrl, settings);
        }

        public async Task<PessoaNomeResponse> GerarPessoaNome()
        {
            var api = Create("http://localhost:5001");
            return await api.GerarPessoaNome();
        }
    }
}