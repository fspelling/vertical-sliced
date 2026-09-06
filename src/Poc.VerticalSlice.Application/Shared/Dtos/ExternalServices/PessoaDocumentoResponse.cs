using System.Text.Json.Serialization;

namespace Poc.VerticalSlice.Application.Shared.Dtos.ExternalServices
{
    public record PessoaDocumentoResponse
    {
        [JsonPropertyName("cpf")]
        public required string Cpf { get; set; }

        [JsonPropertyName("rg")]
        public required string RG { get; set; }
    }
}
