namespace Poc.VerticalSlice.WebApi.Entities
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataCriacaoUtc { get; private set; }

        public Produto(string nome, string? descricao, decimal preco)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Preco = preco;
            Descricao = descricao;
            DataCriacaoUtc = DateTime.UtcNow;
        }
    }
}
