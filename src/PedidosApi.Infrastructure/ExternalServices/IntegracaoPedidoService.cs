using MassTransit;
using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Interfaces;
using PedidosApi.Infrastructure.ExternalServices.Messages;

namespace PedidosApi.Infrastructure.ExternalServices;

public class IntegracaoPedidoService: IIntegracaoPedidoService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public IntegracaoPedidoService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task EnviarPedidoAsync(Pedido? pedido)
    {
        if (pedido == null)
            return;

        await _publishEndpoint.Publish(new PedidoMessage
        {
            Id = Guid.NewGuid(),
            ClienteId = pedido.ClienteId,
            PedidoId = pedido.Id,
            Itens = pedido.Itens
        });
    }
}