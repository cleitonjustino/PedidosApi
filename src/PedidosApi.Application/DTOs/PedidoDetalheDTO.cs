namespace PedidosApi.Application.DTOs;

public record PedidoDetalheDTO(
    int Id,
    int PedidoId,
    int ClienteId,
    decimal Imposto,
    List<ItemDTO> Itens,
    string Status
);
