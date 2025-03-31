using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;
using PedidosApi.Domain.Interfaces;

namespace PedidosApi.Domain.Services;

public class PedidoService(IPedidoRepository pedidoRepository,
    IIntegracaoPedidoService integracaoService,
    IFeatureFlagService featureFlagService) : IPedidoService
{
    private const string FEATURE_REFORMA_TRIBUTARIA = "ReformaTributaria";

    public async Task<Pedido?> CriarPedidoAsync(int pedidoId, int clienteId, List<Item> itens)
    {
        var pedidoExiste = await pedidoRepository.ExistePedidoAsync(pedidoId);
        if (pedidoExiste)
        {
            throw new InvalidOperationException($"Pedido com ID {pedidoId} já existe.");
        }

        var pedido = new Pedido(pedidoId, clienteId, itens);
            
        var usarReformaTributaria = await featureFlagService.IsFeatureEnabledAsync(FEATURE_REFORMA_TRIBUTARIA);
        pedido.CalcularImposto(usarReformaTributaria);
            
        await pedidoRepository.AdicionarAsync(pedido);
        await pedidoRepository.SalvarAsync();
            
        await integracaoService.EnviarPedidoAsync(pedido);
            
        return pedido;
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        return await pedidoRepository.ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Pedido?>> ListarPorStatusAsync(StatusPedido status)
    {
        return await pedidoRepository.ListarPorStatusAsync(status);
    }
}