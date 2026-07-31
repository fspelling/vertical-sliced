using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Poc.VerticalSlice.WebApi.DataBase;
using Poc.VerticalSlice.WebApi.Features.Produto;
using Prometheus;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.UseHttpClientMetrics();

builder.Services.AddDbContext<VsaDbContext>(options =>
    options.UseInMemoryDatabase("VsaDB"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddScoped<CriarProduto.Repository>();
builder.Services.AddScoped<ObterProdutoPorId.Repository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.MapCarter();

app.UseHttpsRedirection();
app.Run();