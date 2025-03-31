using PedidosApi.Domain.Entities;

namespace PedidosApi.Domain.Interfaces;

public interface IIntegracaoPedidoService
{
    Task EnviarPedidoAsync(Pedido? pedido);
}