namespace Poc.VerticalSlice.Application.Shared.Entities;

public class Pessoa(string cpf, string rg, string nome)
{
    public string Cpf { get; private set; } = cpf;
    public string RG { get; private set; } = rg;
    public string Nome { get; private set; } = nome;
}
