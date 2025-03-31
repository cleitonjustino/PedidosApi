using FluentValidation;
using PedidosApi.Application.DTOs;

namespace PedidosApi.Application.Validators;

public class PedidoRequestValidator : AbstractValidator<PedidoRequestDTO>
{
    public PedidoRequestValidator()
    {
        RuleFor(p => p.PedidoId)
            .GreaterThan(0)
            .WithMessage("PedidoId deve ser maior que zero.");

        RuleFor(p => p.ClienteId)
            .GreaterThan(0)
            .WithMessage("ClienteId deve ser maior que zero.");

        RuleFor(p => p.Itens)
            .NotEmpty()
            .WithMessage("Pedido deve conter pelo menos um item.");

        RuleForEach(p => p.Itens).SetValidator(new ItemValidator());
    }
}

public class ItemValidator : AbstractValidator<ItemDTO>
{
    public ItemValidator()
    {
        RuleFor(i => i.ProdutoId)
            .GreaterThan(0)
            .WithMessage("ProdutoId deve ser maior que zero.");

        RuleFor(i => i.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que zero.");

        RuleFor(i => i.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero.");
    }
}