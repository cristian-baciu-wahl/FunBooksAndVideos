using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Rules;

/// <summary>
/// BR1: If the purchase order contains a membership, it has to be activated in the customer account immediately
/// </summary>
public class ActivateMembershipRule(ICustomerMembershipService membershipService) : IBusinessRule
{
    private readonly ICustomerMembershipService _membershipService = membershipService;

    public string RuleId => "BR1_MembershipActivation";

    public int Priority => 10; 

    public bool ShouldApply(PurchaseOrder order)
    {
        return order.ItemLines.Any(item => item.IsMembership);
    }

    public void Apply(PurchaseOrder order)
    {
        var membershipLines = order.ItemLines.Where(item => item.IsMembership);

        foreach (var line in membershipLines)
        {
            if (!line.MembershipType.HasValue) continue;
            _membershipService.ActivateMembership(
                order.CustomerId,
                line.MembershipType.Value);
        }
    }
}

