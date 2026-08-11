using FluentValidation;
using FunBooksAndVideos.API.Models;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Infrastructure.Interfaces;

namespace FunBooksAndVideos.API.Validators;

public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
{
    private readonly ICustomerMembershipService _customerService;
    private readonly IPurchaseOrderRepository _orderRepository;

    public CreatePurchaseOrderRequestValidator(
        ICustomerMembershipService customerService,
        IPurchaseOrderRepository orderRepository)
    {
        _customerService = customerService;
        _orderRepository = orderRepository;

        // Validate Customer ID
        RuleFor(x => x.CustomerId)
            .Must(CustomerExists)
            .WithMessage(x => $"Customer with ID {x.CustomerId} not found");

        // Validate Items List
        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Order must contain at least one item")
            .Must(items => items != null && items.Count != 0)
            .WithMessage("Order must contain at least one item");

        // Validate each item using a specific order item validator
        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestValidator(_orderRepository));
    }

    private bool CustomerExists(int customerId)
    {
        return _customerService.CustomerExists(customerId);
    }
}
