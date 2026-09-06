using Microsoft.Extensions.Configuration;
using Refit;

namespace Poc.VerticalSlice.Application.Shared.ServiceExternal;

public abstract class RefitBase(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    protected static T Create<T>(string baseUrl)
    {
        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            })
        };
        return RestService.For<T>(baseUrl, settings);
    }
}
