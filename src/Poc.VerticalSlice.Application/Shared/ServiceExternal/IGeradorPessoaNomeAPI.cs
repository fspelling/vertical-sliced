using Poc.VerticalSlice.Application.Shared.ServiceExternal.Dtos;
using Refit;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal;

public interface IGeradorPessoaNomeAPI
{
    [Get("/nomecompleto")]
    Task<PessoaNomeResponse> GerarPessoaNome();
}
