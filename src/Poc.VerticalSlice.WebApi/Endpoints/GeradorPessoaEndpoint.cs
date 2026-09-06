using Carter;
using FluentResults;
using MediatR;
using Poc.VerticalSlice.Application.Features.Pessoa;
using Poc.VerticalSlice.Application.Features.Produto;
using Poc.VerticalSlice.WebApi.Extensions;

namespace Poc.VerticalSlice.WebApi.Endpoints;

public class GeradorPessoaEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var endpoint = app.MapGet("api/geradorPessoa", async (ISender sender) =>
        {
            var result = await sender.Send(new GerarPessoa.Query());
            return result!.ToResultCustom();
        });

        ConfigMetadata(endpoint);
    }

    private void ConfigMetadata(RouteHandlerBuilder builder)
    {
        builder
            .WithTags("GeradorPessoa")
            .WithDescription("Gerar uma nova pessoa de forma randomica.")
            .Accepts<CriarProduto.Request>("application/json")
            .Produces<Result<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();
    }
}
