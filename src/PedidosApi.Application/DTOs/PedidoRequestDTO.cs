namespace PedidosApi.Application.DTOs;

public record PedidoRequestDTO(
    int PedidoId,
    int ClienteId,
    List<ItemDTO> Itens
);
