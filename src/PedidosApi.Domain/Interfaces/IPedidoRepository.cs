using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;

namespace PedidosApi.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> AdicionarAsync(Pedido? pedido);
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<Pedido?> ObterPorPedidoIdAsync(int pedidoId);
    Task<IEnumerable<Pedido?>> ListarPorStatusAsync(StatusPedido status);
    Task<bool> ExistePedidoAsync(int pedidoId);
    Task SalvarAsync();
}