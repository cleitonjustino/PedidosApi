using PedidosApi.Domain.Enums;

namespace PedidosApi.Domain.Entities;

public class Pedido
{
    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public int ClienteId { get; private set; }
    public decimal Imposto { get; private set; }
    public List<Item> Itens { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTime DataCriacao { get; private set; }

    public Pedido(int pedidoId, int clienteId, List<Item> itens)
    {
        ValidarPedido(pedidoId, clienteId, itens);
            
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Itens = itens;
        Status = StatusPedido.Criado;
        DataCriacao = DateTime.UtcNow;
    }

    private void ValidarPedido(int pedidoId, int clienteId, List<Item> itens)
    {
        if (pedidoId <= 0)
            throw new ArgumentException("PedidoId deve ser maior que zero.", nameof(pedidoId));
            
        if (clienteId <= 0)
            throw new ArgumentException("ClienteId deve ser maior que zero.", nameof(clienteId));
            
        if (itens == null || !itens.Any())
            throw new ArgumentException("Pedido deve conter pelo menos um item.", nameof(itens));
    }

    public void CalcularImposto(bool usarReformaTributaria)
    {
        var valorTotalItens = Itens.Sum(item => item.ValorTotal);
        var taxaImposto = usarReformaTributaria ? 0.2m : 0.3m;
            
        Imposto = valorTotalItens * taxaImposto;
    }

    public void AtualizarStatus(StatusPedido novoStatus)
    {
        Status = novoStatus;
    }
}