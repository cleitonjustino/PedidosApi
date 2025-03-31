using PedidosApi.Application.DTOs;
using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;

namespace PedidosApi.Application.Mappers;

public static class PedidoMapper
{
    public static Pedido ToDomain(this PedidoRequestDTO pedidoDTO)
    {
        return new Pedido(
            pedidoDTO.PedidoId,
            pedidoDTO.ClienteId,
            pedidoDTO.Itens.Select(i => new Item
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                Valor = i.Valor
            }).ToList()
        );
    }

    public static PedidoResponseDTO ToResponseDTO(this Pedido pedido)
    {
        return new PedidoResponseDTO(pedido.Id, pedido.Status.ToString());
    }

    public static PedidoDetalheDTO? ToDetalheDTO(this Pedido pedido)
    {
        return new PedidoDetalheDTO(
            pedido.Id,
            pedido.PedidoId,
            pedido.ClienteId,
            pedido.Imposto,
            pedido.Itens.Select(i => new ItemDTO(i.ProdutoId, i.Quantidade, i.Valor)).ToList(),
            pedido.Status.ToString());
   }

    public static List<PedidoDetalheDTO>? ToDetalheListDTO(this IEnumerable<Pedido> pedidos)
    {
        return pedidos.Select(p => p.ToDetalheDTO()).ToList();
    }

    public static StatusPedido ToEnum(this string status)
    {
        return Enum.TryParse<StatusPedido>(status, true, out var result) 
            ? result : StatusPedido.Criado;
    }
}