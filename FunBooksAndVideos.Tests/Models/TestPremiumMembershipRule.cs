using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Tests.Models
{
    public class TestPremiumMembershipRule(ICustomerMembershipService membershipService) : IBusinessRule
    {
        public int Priority => 999;

        public string RuleId => "BR999_PremiumMembershipActivation";

        public RuleExecutionStage Stage => RuleExecutionStage.PreProcessing;

        public void Apply(PurchaseOrder order)
        {
            membershipService.ActivateMembership(order.CustomerId, MembershipType.Premium);
        }

        public bool ShouldApply(PurchaseOrder order)
        {
            return true;
        }
    }
}
