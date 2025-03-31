namespace PedidosApi.Domain.Entities;

public class Item
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorTotal => Quantidade * Valor;
}