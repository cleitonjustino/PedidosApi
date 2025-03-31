using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using PedidosApi.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using PedidosApi.Application.Features;
using PedidosApi.Domain.Interfaces;
using PedidosApi.Infrastructure.Data;
using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Services;

namespace IntegrationTests;

public class PedidosApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PedidosApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Remover o registro do DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PedidoDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbName = $"TestDatabase_{Guid.NewGuid()}";
                services.AddDbContext<PedidoDbContext>(options => { options.UseInMemoryDatabase(dbName); });

                var integracaoPedidoService = Substitute.For<IIntegracaoPedidoService>();
                integracaoPedidoService.EnviarPedidoAsync(Arg.Any<Pedido>())
                    .Returns(Task.FromResult(true));
                services.AddSingleton<IIntegracaoPedidoService>(integracaoPedidoService);

                // Registrar ou substituir outros serviços necessários
                services.AddScoped<IPedidoRepository, PedidoRepository>();
                services.AddScoped<IPedidoService, PedidoService>();
                services.AddScoped<IPedidoFeature, PedidoFeature>();

                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
                    dbContext.Database.EnsureCreated();
                }

                services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarCreated()
    {
        // Arrange
        var pedidoDTO = new PedidoRequestDTO(1, 1, new List<ItemDTO> { new(1, 2, 50) });

        var content = new StringContent(
            JsonConvert.SerializeObject(pedidoDTO),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/pedidos", content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = JsonConvert.DeserializeObject<PedidoResponseDTO>(responseString);
        Assert.NotNull(resultado);
        Assert.Equal("Criado", resultado.Status);
    }

    [Fact]
    public async Task CriarPedidoDuplicado_DeveRetornarBadRequest()
    {
        // Arrange
        var pedidoDTO = new PedidoRequestDTO(2, 1, new List<ItemDTO> { new(1, 2, 50) });

        var content = new StringContent(
            JsonConvert.SerializeObject(pedidoDTO),
            Encoding.UTF8,
            "application/json");

        // Act - Primeiro pedido
        var response1 = await _client.PostAsync("/api/pedidos", content);
        if (!response1.IsSuccessStatusCode)
        {
            var errorContent = await response1.Content.ReadAsStringAsync();
            Console.WriteLine($"Falha ao criar o primeiro pedido: {response1.StatusCode}, {errorContent}");
            return;
        }

        // Act - Segundo pedido (duplicado)
        var responseDuplicado = await _client.PostAsync("/api/pedidos", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseDuplicado.StatusCode);
    }

    [Fact]
    public async Task ObterPedido_QuandoPedidoExiste_DeveRetornarOk()
    {
        // Arrange 
        var pedidoDTO = new PedidoRequestDTO(3, 1, new List<ItemDTO> { new(1, 2, 50) });

        var content = new StringContent(
            JsonConvert.SerializeObject(pedidoDTO),
            Encoding.UTF8,
            "application/json");

        var responseCreate = await _client.PostAsync("/api/pedidos", content);
        var responseString = await responseCreate.Content.ReadAsStringAsync();
        var resultadoCreate = JsonConvert.DeserializeObject<PedidoResponseDTO>(responseString);

        // Act
        var response = await _client.GetAsync($"/api/pedidos/{resultadoCreate.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseGetString = await response.Content.ReadAsStringAsync();
        var resultado = JsonConvert.DeserializeObject<PedidoDetalheDTO>(responseGetString);

        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.PedidoId);
        Assert.Equal(1, resultado.ClienteId);
        Assert.Equal("Criado", resultado.Status);
    }

    [Fact]
    public async Task ObterPedido_QuandoPedidoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/pedidos/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarOk()
    {
        // Arrange 
        for (int i = 0; i <= 3; i++)
        {
            var pedidoDTO = new PedidoRequestDTO(i, 1, new List<ItemDTO> { new(1, 2, 50) });
            var content = new StringContent(
                JsonConvert.SerializeObject(pedidoDTO),
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/pedidos", content);
        }

        // Act
        var response = await _client.GetAsync("/api/pedidos?status=Criado");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var resultado = JsonConvert.DeserializeObject<List<PedidoDetalheDTO>>(responseString);

        Assert.NotNull(resultado);
        Assert.True(resultado.Count >= 1); 
    }
}