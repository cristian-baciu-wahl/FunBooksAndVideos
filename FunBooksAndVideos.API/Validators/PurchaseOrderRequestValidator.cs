using FluentValidation;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.API.Validators;

public class PurchaseOrderRequestValidator: AbstractValidator<PurchaseOrderRequest>
{
    public PurchaseOrderRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than 0");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                // A line item must be either a product, or a membership
                item.RuleFor(x => x.ProductId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Product ID must be greater than 0")
                    .When(x => string.IsNullOrWhiteSpace(x.MembershipType));

                item.RuleFor(x => x.MembershipType)
                    .NotEmpty()
                    .WithMessage("Membership type is required")
                    .When(x => x.ProductId == null);

                item.RuleFor(x => x.MembershipType)
                    .Must(BeValidMembershipType)
                    .WithMessage("Invalid membership type")
                    .When(x => !string.IsNullOrWhiteSpace(x.MembershipType));

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Unit price cannot be negative");
            });
    }

    private static bool BeValidMembershipType(string? membershipType)
    {
        return Enum.TryParse<MembershipType>(
            membershipType,
            ignoreCase: true,
            out _);
    }
}