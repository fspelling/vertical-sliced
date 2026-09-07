using Poc.VerticalSlice.Application.Shared.ServiceExternal.Dtos;
using Refit;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal;

public interface IGeradorPessoaDocumentoAPI
{
    [Get("/documento")]
    Task<PessoaDocumentoResponse> GerarPessoaDocumento();
}
