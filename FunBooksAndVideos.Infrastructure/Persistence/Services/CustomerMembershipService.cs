using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Infrastructure.Persistence.Services;

public sealed class CustomerMembershipService(AppDbContext dbContext)
    : ICustomerMembershipService
{
    public void ActivateMembership(int customerId, MembershipType membershipType)
    {
        var customer = dbContext.Customers
            .Include(existingCustomer => existingCustomer.Memberships)
            .SingleOrDefault(existingCustomer => existingCustomer.Id == customerId)
            ?? throw new ArgumentException($"Customer with ID {customerId} was not found.");

        customer.ActivateMembership(membershipType);
    }
}