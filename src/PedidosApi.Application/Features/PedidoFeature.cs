using PedidosApi.Application.DTOs;
using PedidosApi.Application.Mappers;
using PedidosApi.Domain.Services;

namespace PedidosApi.Application.Features;

public class PedidoFeature : IPedidoFeature
{
    private readonly PedidoService _pedidoService;

    public PedidoFeature(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    public async Task<PedidoResponseDTO> CriarPedidoAsync(PedidoRequestDTO pedidoDTO)
    {
        try
        {
            var pedido = await _pedidoService.CriarPedidoAsync(
                pedidoDTO.PedidoId,
                pedidoDTO.ClienteId,
                pedidoDTO.Itens.Select(i => new Domain.Entities.Item
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    Valor = i.Valor
                }).ToList()
            );

            return pedido.ToResponseDTO();
        }
        catch (Exception ex)
        {
            // Logging seria feito aqui
            throw;
        }
    }

    public async Task<PedidoDetalheDTO?> ObterPedidoAsync(int id)
    {
        var pedido = await _pedidoService.ObterPorIdAsync(id);
            
        return pedido?.ToDetalheDTO();
    }

    public async Task<IEnumerable<PedidoDetalheDTO?>> ListarPedidosPorStatusAsync(string status)
    {
        var statusEnum = status.ToEnum();
        var pedidos = await _pedidoService.ListarPorStatusAsync(statusEnum);
        return pedidos.ToDetalheListDTO();
    }
}