using FluentValidation;
using Poc.VerticalSlice.WebApi.Config;
using Poc.VerticalSlice.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services
    .AddCarterConfiguration()
    .AddMetricsConfiguration()
    .AddDatabaseConfiguration()
    .AddIocConfiguration(assembly);

builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseScalarConfiguration();

app.UseMetricsConfiguration();
app.UseCarterConfiguration();

app.UseHttpsRedirection();
app.Run();