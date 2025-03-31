using Microsoft.EntityFrameworkCore;
using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;
using PedidosApi.Domain.Interfaces;

namespace PedidosApi.Infrastructure.Data;

public class PedidoRepository(PedidoDbContext context) : IPedidoRepository
{
    public async Task<Pedido?> AdicionarAsync(Pedido? pedido)
    {
        await context.Pedidos.AddAsync(pedido);
        return pedido;
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        return await context.Pedidos.FindAsync(id);
    }

    public async Task<Pedido?> ObterPorPedidoIdAsync(int pedidoId)
    {
        return await context.Pedidos.FirstOrDefaultAsync(p => p!.PedidoId == pedidoId);
    }

    public async Task<IEnumerable<Pedido?>> ListarPorStatusAsync(StatusPedido status)
    {
        return await context.Pedidos
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<bool> ExistePedidoAsync(int pedidoId)
    {
        return await context.Pedidos.AnyAsync(p => p!.PedidoId == pedidoId);
    }

    public async Task SalvarAsync()
    {
        await context.SaveChangesAsync();
    }
}