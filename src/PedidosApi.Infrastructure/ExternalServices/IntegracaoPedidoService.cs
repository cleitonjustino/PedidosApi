using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Interfaces;

namespace PedidosApi.Infrastructure.ExternalServices;

public class IntegracaoPedidoService: IIntegracaoPedidoService
{
    public Task EnviarPedidoAsync(Pedido? pedido)
    {
        throw new NotImplementedException();
    }
}