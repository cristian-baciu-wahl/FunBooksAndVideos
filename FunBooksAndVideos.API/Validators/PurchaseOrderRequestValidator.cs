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
                item.RuleFor(x => x)
                    .Must(ContainExactlyOnePurchaseType)
                    .WithMessage("Each item must contain either a product or a membership, but not both.");

                item.RuleFor(x => x.ProductId)
                    .GreaterThan(0)
                    .WithMessage("Product ID must be greater than 0")
                    .When(x => x.ProductId.HasValue);

                item.RuleFor(x => x.MembershipType)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Membership type is required")
                    .Must(BeValidMembershipType)
                    .WithMessage("Invalid membership type")
                    .When(x => x.MembershipType is not null);
            });
    }

    private static bool BeValidMembershipType(string? membershipType)
    {
        return Enum.TryParse<MembershipType>(
            membershipType,
            ignoreCase: true,
            out _);
    }

    private static bool ContainExactlyOnePurchaseType(PurchaseOrderItemRequest item)
    {
        var hasProduct = item.ProductId.HasValue;
        var hasMembership = item.MembershipType is not null;

        return hasProduct ^ hasMembership;
    }
}