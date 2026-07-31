using Carter;
using FluentValidation;
using Poc.VerticalSlice.WebApi.Config;
using Poc.VerticalSlice.WebApi.Configurations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCarter();

builder.Services
    .AddMetricsConfiguration()
    .AddDatabaseConfiguration()
    .AddDependencyInjectionConfiguration(assembly);

builder.Services.AddValidatorsFromAssembly(assembly);

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