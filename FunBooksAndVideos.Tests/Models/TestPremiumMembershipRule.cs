using FunBooksAndVideos.Application.BusinessRules;
using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Tests.Models
{
    public class TestPremiumMembershipRule(ICustomerMembershipService membershipService) : IBusinessRule
    {
        public int Priority => 999;

        public string RuleId => "BR999_PremiumMembershipActivation";

        public RuleExecutionStage Stage => RuleExecutionStage.PreProcessing;


        public Task ApplyAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
        {
            membershipService.ActivateMembership(order.CustomerId, MembershipType.Premium);
            return Task.CompletedTask;
        }

        public bool ShouldApply(PurchaseOrder order)
        {
            return true;
        }
    }
}
