using FluentResults;

namespace Poc.VerticalSlice.WebApi.Extensions;

public static class ResultExtension
{
    public static IResult ToResultCustom<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result.Errors);
    }
}
