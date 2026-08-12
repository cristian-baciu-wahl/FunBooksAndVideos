using FunBooksAndVideos.Domain.Customers;

namespace FunBooksAndVideos.Application.BusinessRules.Ports;

public interface ICustomerMembershipService
{
    void ActivateMembership(int customerId, MembershipType membershipType);
    bool HasActiveMembership(int customerId, MembershipType membershipType);
    bool CustomerExists(int customerId);
    Customer? GetCustomer(int customerId);
}