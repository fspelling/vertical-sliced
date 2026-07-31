using FluentResults;

namespace Poc.VerticalSlice.WebApi.Shared.Extensions
{
    public static class ResultExtension
    {
        public static IResult ToResultCustom<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result.Errors);
        }
    }

}
