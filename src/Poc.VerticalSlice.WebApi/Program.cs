using Carter;
using FluentValidation;
using Poc.VerticalSlice.WebApi.Config;
using Poc.VerticalSlice.WebApi.Configurations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCarter();

builder.Services
    .AddMetricsConfiguration()
    .AddDatabaseConfiguration()
    .AddDependencyInjectionConfiguration()
    .AddTracerConfiguration(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCarter();
app.UseMetricsConfiguration();

app.UseHttpsRedirection();
app.Run();