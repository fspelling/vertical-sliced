# 🧩 PoC - Vertical Slice Architecture (.NET 9)

Este projeto é uma **Prova de Conceito (PoC)** desenvolvida em **.NET 9**, com o objetivo de demonstrar a aplicação prática do padrão **Vertical Slice Architecture**, aliado a princípios do **SOLID** e boas práticas como **injeção de dependências**, **validações**, **CQRS** e **repositórios isolados**.

---

## 📐 Arquitetura

A aplicação segue o padrão **Vertical Slice Architecture**, onde **cada feature é isolada** em seu próprio diretório, contendo toda a lógica necessária (Command/Query, Handler, Validator, Repository, DTOs, etc).

### Estrutura geral:
```
Poc.VSA.Api/
├── DataBase/
│   └── VsaDbContext.cs
├── Entities/
│   └── Produto.cs
├── Features/
│   └── Produto/
│       ├── CriarProduto.cs
│       └── ObterProdutoPorId.cs
├── Shared/
│   └── Extensions/
│       └── ResultExtension.cs
├── Program.cs
└── appsettings.json
```

### 📁 Explicação dos diretórios
- **DataBase/** → contém o `DbContext` e a configuração de persistência.
- **Entities/** → modelos de domínio da aplicação.
- **Features/** → contém os "slices" da aplicação, um por funcionalidade.
- **Shared/** → classes utilitárias e extensões compartilhadas entre as features.

---

## ⚙️ Tecnologias e Bibliotecas

| Tecnologia | Função |
|-------------|--------|
| **.NET 8 Minimal API** | Estrutura principal da aplicação |
| **Carter** | Simplificação de rotas e endpoints |
| **FluentValidation** | Validação de comandos |
| **MediatR** | Implementação do padrão CQRS |
| **Mapster** | Mapeamento entre DTOs e entidades |
| **EF Core Memory** | Acesso ao banco de dados |
| **SQL Server** | Banco de dados relacional |
| **Result Pattern (FluentResults)** | Padronização de retornos |
| **Scalar** | Ferramenta de documentacao para a API |

---

## 🚀 Como executar o projeto

### 1️⃣ Clonar o repositório
```bash
git clone https://github.com/fspelling/Poc.VSA.git
cd Poc.VSA
```

### 2️⃣ Rodar a aplicação
```bash
dotnet run
```

A API será iniciada em:
```
http://localhost:7157
```

---

## 🔍 Endpoints principais

| Método | Endpoint | Descrição |
|--------|-----------|-----------|
| `POST` | `/api/produtos` | Cria um novo produto |
| `GET`  | `/api/produtos/{id}` | Obtém um produto pelo ID |

---

## 🧠 Conceitos aplicados

- **Alta coesão e baixo acoplamento entre as fatias (slices)**
- **Separação de comandos e consultas (CQRS)**

Cada “slice” (`CriarProduto`, `ObterProdutoPorId`, etc.) é independente e autossuficiente, contendo:
- **Handler (lógica principal)**
- **Repository (acesso a dados)**
- **Command/Query (contrato de entrada)**
- **Validator (regras de negócio)**
- **Endpoint (ponto de exposição da API)**

---

## 🧩 Exemplo de fluxo - Criar Produto

1. O endpoint `CriarProduto` recebe a requisição.
2. O objeto `Request` é validado com `FluentValidation`.
3. O `Command` é enviado via `MediatR`.
4. O `Handler` executa a lógica e usa o `Repository` para persistir no banco.
5. O retorno é padronizado com `ResultExtension`.

---

## 💡 Objetivo da PoC

Demonstrar como é possível **organizar uma aplicação .NET moderna** de forma **modular, escalável e independente**, permitindo evoluir cada funcionalidade sem impactar as demais.

---

## 🧑‍💻 Autor

**Fernando Spelling**  
💬 Estudando arquitetura de software, CQRS, SOLID, Clean Architecture e práticas DevOps.  

---
