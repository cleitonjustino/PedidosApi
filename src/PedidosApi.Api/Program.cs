using ConfigCat.Client;
using Microsoft.EntityFrameworkCore;
using PedidosApi.Application.Features;
using PedidosApi.Application.Validators;
using PedidosApi.Domain.Interfaces;
using PedidosApi.Domain.Services;
using PedidosApi.Infrastructure.Data;
using PedidosApi.Infrastructure.ExternalServices;
using PedidosApi.Middleware;
using Serilog;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Banco de dados
builder.Services.AddDbContext<PedidoDbContext>(options =>
    options.UseInMemoryDatabase("PedidosDb"));

//ConfigCat
var configCatSdkKey = builder.Configuration["ConfigCatSdkKey"];

builder.Services.AddSingleton<IConfigCatClient>(sp => ConfigCatClient.Get(configCatSdkKey!));

// Registro de serviços e repositories
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddSingleton<IFeatureFlagService, FeatureFlagService>();
builder.Services.AddScoped<IIntegracaoPedidoService, IntegracaoPedidoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<PedidoFeature>();



// Registrar validadores
builder.Services.AddScoped<PedidoRequestValidator>();

var app = builder.Build(); 

app.UseMiddleware<ExceptionHandlerMiddleware>();

// app.UseHttpsRedirection();
// app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.UseSwagger();
app.UseSwagger(); // Ativa a geração do Swagger
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API V1");
});

app.Run();

public partial class Program { }
