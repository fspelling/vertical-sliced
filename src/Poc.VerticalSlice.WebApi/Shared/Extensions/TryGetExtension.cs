namespace Poc.VerticalSlice.WebApi.Shared.Extensions
{
    public static class TryGetExtension
    {
        public static bool TryGetIdempotencyKey(this HttpContext context, out Guid idempotenceKey)
        {
            idempotenceKey = Guid.Empty;

            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var headerValue))
                return false;

            Guid.TryParse(headerValue, out var parsedKey);
            idempotenceKey = parsedKey;

            return true;
        }
    }
}
