using FluentAssertions;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using PedidosApi.Application.DTOs;
using PedidosApi.Application.Features;
using PedidosApi.Controllers;

namespace UnitTests.Controllers;

public class PedidosControllerTests
{
    private readonly IPedidoFeature _pedidoFeature;
    private readonly ILogger<PedidosController> _logger;
    private readonly PedidosController _controller;

    public PedidosControllerTests()
    {
        _pedidoFeature = Substitute.For<IPedidoFeature>();
        _logger = Substitute.For<ILogger<PedidosController>>();
        _controller = new PedidosController(_pedidoFeature, _logger);
    }

    [Fact]
    public async Task CriarPedido_QuandoSucesso_DeveRetornarCreatedAtAction()
    {
        // Arrange
        var pedidoDTO = new PedidoRequestDTO(1, 1, new List<ItemDTO> { new(1, 2, 50) });

        var pedidoResponseDTO = new PedidoResponseDTO(1, "Criado");
        _pedidoFeature.CriarPedidoAsync(pedidoDTO).Returns(pedidoResponseDTO);

        // Act
        var resultado = await _controller.CriarPedido(pedidoDTO);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = resultado as CreatedAtActionResult;
        createdAtResult.ActionName.Should().Be("ObterPedido");
        createdAtResult.RouteValues["id"].Should().Be(1);
        createdAtResult.Value.Should().Be(pedidoResponseDTO);
    }

    [Fact]
    public async Task CriarPedido_QuandoPedidoDuplicado_DeveRetornarBadRequest()
    {
        // Arrange
        var pedidoDTO = new PedidoRequestDTO(1, 1, new List<ItemDTO> { new(1, 2, 50) });

        _pedidoFeature.CriarPedidoAsync(pedidoDTO).Throws(new InvalidOperationException("Pedido com ID 1 já existe."));

        // Act
        var resultado = await _controller.CriarPedido(pedidoDTO);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterPedido_QuandoPedidoExiste_DeveRetornarOk()
    {
        // Arrange
        var pedidoDetalheDTO = new PedidoDetalheDTO(1, 1, 1, 1, new List<ItemDTO> { new(1, 2, 50) }, "Criado");

        _pedidoFeature.ObterPedidoAsync(1).Returns(pedidoDetalheDTO);

        // Act
        var resultado = await _controller.ObterPedido(1);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult.Value.Should().Be(pedidoDetalheDTO);
    }

    [Fact]
    public async Task ObterPedido_QuandoPedidoNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        _pedidoFeature.ObterPedidoAsync(1).Returns((PedidoDetalheDTO)null);

        // Act
        var resultado = await _controller.ObterPedido(1);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarOk()
    {
        // Arrange
        var pedidos = new List<PedidoDetalheDTO>
        {
            new(1, 1, 1, 30, new List<ItemDTO> { new(1, 2, 50) }, "Criado"),
            new(2, 2, 1, 20, new List<ItemDTO> { new(2, 1, 100) }, "Criado")
        };

        _pedidoFeature.ListarPedidosPorStatusAsync("Criado").Returns(pedidos);

        // Act
        var resultado = await _controller.ListarPedidos("Criado");

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult.Value.Should().Be(pedidos);
    }
}