using ConfigCat.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PedidosApi.Application.Features;
using PedidosApi.Application.Validators;
using PedidosApi.Domain.Interfaces;
using PedidosApi.Domain.Services;
using PedidosApi.Infrastructure.Data;
using PedidosApi.Infrastructure.ExternalServices;
using PedidosApi.Middleware;
using Serilog;
using PedidosApi.Infrastructure.Consumers;

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
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();
builder.Services.AddScoped<IIntegracaoPedidoService, IntegracaoPedidoService>();
builder.Services.AddScoped<IPedidoFeature, PedidoFeature>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PedidoMessageConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5672, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("pedido-queue", e =>
        {
            e.ConfigureConsumer<PedidoMessageConsumer>(context);
        });
    });
});

// Registrar validadores
builder.Services.AddScoped<PedidoRequestValidator>();

var app = builder.Build(); 

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.UseSwagger();
app.UseSwagger(); 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API V1");
});

app.Run();

public partial class Program { }
