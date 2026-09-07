using System.Text.Json.Serialization;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal.Dtos;

public record PessoaNomeResponse
{
    [JsonPropertyName("nomeCompleto")]
    public required string Nome { get; set; }
}
