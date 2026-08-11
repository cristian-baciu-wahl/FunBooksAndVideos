using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface ICustomerMembershipService
{
    void ActivateMembership(int customerId, MembershipType membershipType);
    bool HasActiveMembership(int customerId, MembershipType membershipType);
    bool CustomerExists(int customerId);
    Customer? GetCustomer(int customerId);
}