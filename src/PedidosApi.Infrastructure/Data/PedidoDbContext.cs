using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PedidosApi.Domain.Entities;

namespace PedidosApi.Infrastructure.Data;

public class PedidoDbContext : DbContext
{
    public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido?> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PedidoId).IsRequired();
            entity.Property(e => e.ClienteId).IsRequired();
            entity.Property(e => e.Imposto).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.Itens)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<Item>>(v, new JsonSerializerOptions())
                );
        });

        base.OnModelCreating(modelBuilder);
    }
}