using Microsoft.AspNetCore.Mvc;
using PedidosApi.Application.DTOs;
using PedidosApi.Application.Features;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoFeature _pedidoFeature;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoFeature pedidoFeature, ILogger<PedidosController> logger)
    {
        _pedidoFeature = pedidoFeature;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequestDTO pedidoDTO)
    {
        try
        {
            _logger.LogInformation("Recebido novo pedido: {PedidoId}", pedidoDTO.PedidoId);
            var resultado = await _pedidoFeature.CriarPedidoAsync(pedidoDTO);
            return CreatedAtAction(nameof(ObterPedido), new { id = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("já existe"))
        {
            _logger.LogWarning(ex, "Tentativa de criar pedido duplicado: {PedidoId}", pedidoDTO.PedidoId);
            return BadRequest(new { erro = "Pedido duplicado", mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido: {PedidoId}", pedidoDTO.PedidoId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { erro = "Erro interno", mensagem = "Ocorreu um erro ao processar o pedido" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PedidoDetalheDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPedido(int id)
    {
        try
        {
            _logger.LogInformation("Consultando pedido: {Id}", id);
            var pedido = await _pedidoFeature.ObterPedidoAsync(id);

            if (pedido == null)
            {
                _logger.LogWarning("Pedido não encontrado: {Id}", id);
                return NotFound(new { erro = "Pedido não encontrado" });
            }

            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { erro = "Erro interno", mensagem = "Ocorreu um erro ao consultar o pedido" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PedidoDetalheDTO[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPedidos([FromQuery] string status = "Criado")
    {
        try
        {
            _logger.LogInformation("Listando pedidos com status: {Status}", status);
            var pedidos = await _pedidoFeature.ListarPedidosPorStatusAsync(status);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos com status: {Status}", status);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { erro = "Erro interno", mensagem = "Ocorreu um erro ao listar os pedidos" });
        }
    }
}