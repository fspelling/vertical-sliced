namespace Poc.VerticalSlice.Application.Shared.Entities;

public class Produto(string nome, string? descricao, decimal preco)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = nome;
    public string? Descricao { get; private set; } = descricao;
    public decimal Preco { get; private set; } = preco;
    public DateTime DataCriacaoUtc { get; private set; } = DateTime.UtcNow;
}
