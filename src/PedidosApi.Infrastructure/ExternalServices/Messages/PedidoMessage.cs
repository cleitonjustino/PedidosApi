using PedidosApi.Domain.Entities;

namespace PedidosApi.Infrastructure.ExternalServices.Messages;

public class PedidoMessage
{
    public Guid Id { get; set; }
    public int ClienteId { get; set; } 
    public int PedidoId { get; set; }
    public List<Item> Itens { get; set; }
}