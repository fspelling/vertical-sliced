using System.Text.Json.Serialization;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal.Dtos;

public record PessoaDocumentoResponse
{
    [JsonPropertyName("cpf")]
    public required string Cpf { get; set; }

    [JsonPropertyName("rg")]
    public required string RG { get; set; }
}
