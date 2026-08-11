using FluentValidation;
using FunBooksAndVideos.API.Models;
using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Infrastructure.Interfaces;

namespace FunBooksAndVideos.API.Validators;

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    private readonly IPurchaseOrderRepository _orderRepository;

    public OrderItemRequestValidator(IPurchaseOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        // Validate ProductId or MembershipType
        RuleFor(x => x)
            .Must(x => x.ProductId.HasValue || !string.IsNullOrEmpty(x.MembershipType))
            .WithMessage("Each item must have either a ProductId or MembershipType");

        // Validate ProductId if provided
        RuleFor(x => x.ProductId)
            .Must(ProductExists)
            .When(x => x.ProductId.HasValue)
            .WithMessage(x => $"Product with ID {x.ProductId} not found");

        // Validate MembershipType if provided
        RuleFor(x => x.MembershipType)
            .Must(BeValidMembershipType)
            .When(x => !string.IsNullOrEmpty(x.MembershipType))
            .WithMessage(x =>
                $"Invalid membership type: '{x.MembershipType}'. " +
                $"Valid values are: {string.Join(", ", Enum.GetNames<MembershipType>())}");

        // Validate Quantity
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");

        // Validate UnitPrice
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative");
    }

    private bool ProductExists(int? productId)
    {
        return productId.HasValue && _orderRepository.ProductExists(productId.Value);
    }

    private bool BeValidMembershipType(string membershipType)
    {
        return Enum.TryParse<MembershipType>(membershipType, true, out _);
    }
}
