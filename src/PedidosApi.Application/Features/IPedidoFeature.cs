using PedidosApi.Application.DTOs;

namespace PedidosApi.Application.Features;

public interface IPedidoFeature
{
    Task<PedidoResponseDTO> CriarPedidoAsync(PedidoRequestDTO dto);
    Task<PedidoDetalheDTO?> ObterPedidoAsync(int id);
    Task<IEnumerable<PedidoDetalheDTO?>> ListarPedidosPorStatusAsync(string status);
}