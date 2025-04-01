using MassTransit;
using PedidosApi.Infrastructure.ExternalServices.Messages;

namespace PedidosApi.Infrastructure.Consumers;

public class PedidoMessageConsumer : IConsumer<PedidoMessage>
{
    public Task Consume(ConsumeContext<PedidoMessage> context)
    {
        var message = context.Message;

        Console.WriteLine($"Pedido recebido: {message.PedidoId}");

        return Task.CompletedTask;
    }
}