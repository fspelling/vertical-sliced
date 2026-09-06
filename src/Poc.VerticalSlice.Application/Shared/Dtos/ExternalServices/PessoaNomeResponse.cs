using System.Text.Json.Serialization;

namespace Poc.VerticalSlice.Application.Shared.Dtos.ExternalServices
{
    public record PessoaNomeResponse
    {
        [JsonPropertyName("nomeCompleto")]
        public required string Nome { get; set; }
    }
}
