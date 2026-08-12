
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

    public RuleExecutionStage Stage => RuleExecutionStage.PreProcessing;

    public int Priority => 10; 

    public bool ShouldApply(PurchaseOrder order)
    {
        return order.ItemLines.OfType<MembershipOrderLine>().Any();
    }

    public void Apply(PurchaseOrder order)
    {
        var membershipLines = order.ItemLines.OfType<MembershipOrderLine>().ToList();
        foreach (var line in membershipLines)
        {
            _membershipService.ActivateMembership(order.CustomerId, line.MembershipType);
        }
    }
}

