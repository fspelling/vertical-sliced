using Microsoft.Extensions.Caching.Distributed;
using Poc.VerticalSlice.WebApi.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poc.VerticalSlice.WebApi.Endpoints.Filters;

public sealed class IdempotencyFilter(int cacheTime = 60) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.TryGetIdempotencyKey(out Guid idempotenceKey))
            return Results.BadRequest("Chave 'Idempotence-Key' inválido ou ausente");

        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        var cacheKey = $"Idempotent_{idempotenceKey}";
        var cacheResult = await GetCacheObject(cache, cacheKey);

        if (cacheResult is not null)
            return cacheResult;

        object? result = await next(context);

        if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeResult and IValueHttpResult valueResult)
            await SetCacheObject(cache, cacheKey, statusCodeResult.StatusCode, valueResult.Value, cacheTime);

        return result;
    }

    private async Task<IdempotentResult?> GetCacheObject(IDistributedCache cacheService, string cacheKey)
    {
        var cacheResult = await cacheService.GetStringAsync(cacheKey);

        if (cacheResult is null)
            return null;

        var response = JsonSerializer.Deserialize<IdempotentResponse>(cacheResult)!;
        return new IdempotentResult(response.StatusCode, response.Value);
    }

    private async Task SetCacheObject(IDistributedCache cacheService, string cacheKey, int? statusCodeResult, object? valueResult, int cacheTime)
    {
        int statusCode = statusCodeResult ?? StatusCodes.Status200OK;
        IdempotentResponse idempotentResponse = new(statusCode, valueResult);

        await cacheService.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(idempotentResponse), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime)
            }
        );
    }
}

public sealed class IdempotentResponse
{
    [JsonConstructor]
    public IdempotentResponse(int statusCode, object? value)
    {
        StatusCode = statusCode;
        Value = value;
    }

    public int StatusCode { get; }
    public object? Value { get; }
}

public sealed class IdempotentResult : IResult
{
    private readonly int _statusCode;
    private readonly object? _value;

    public IdempotentResult(int statusCode, object? value)
    {
        _statusCode = statusCode;
        _value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = _statusCode;
        return httpContext.Response.WriteAsJsonAsync(_value);
    }
}
