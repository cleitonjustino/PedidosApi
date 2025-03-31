using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;

namespace PedidosApi.Domain.Interfaces;

public interface IPedidoService
{
    Task<Pedido?> CriarPedidoAsync(int pedidoId, int clienteId, List<Item> itens);
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<IEnumerable<Pedido?>> ListarPorStatusAsync(StatusPedido status);
}