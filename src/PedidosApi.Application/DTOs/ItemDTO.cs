namespace PedidosApi.Application.DTOs;

public record ItemDTO(
    int ProdutoId,
    int Quantidade,
    decimal Valor
);