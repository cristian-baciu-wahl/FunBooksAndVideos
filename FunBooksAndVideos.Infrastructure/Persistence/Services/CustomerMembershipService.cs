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

        if (!customer.IsActive)
            throw new InvalidOperationException($"Customer {customerId} is not active.");

        Activate(customer, membershipType);

        if (membershipType == MembershipType.Premium)
        {
            Activate(customer, MembershipType.BookClub);
            Activate(customer, MembershipType.VideoClub);
        }
    }

    public bool HasActiveMembership(int customerId, MembershipType membershipType) =>
        dbContext.Memberships.Any(membership =>
            membership.CustomerId == customerId &&
            membership.Type == membershipType &&
            membership.IsActive);

    public bool CustomerExists(int customerId) =>
        dbContext.Customers.Any(customer => customer.Id == customerId);

    public Customer? GetCustomer(int customerId) =>
        dbContext.Customers
            .Include(customer => customer.Memberships)
            .SingleOrDefault(customer => customer.Id == customerId);

    private static void Activate(Customer customer, MembershipType membershipType)
    {
        var existingMembership = customer.Memberships
            .SingleOrDefault(membership => membership.Type == membershipType);

        if (existingMembership is not null)
        {
            existingMembership.IsActive = true;
            existingMembership.ActivationDate = DateTime.UtcNow;
            return;
        }

        customer.Memberships.Add(new Membership(membershipType)
        {
            CustomerId = customer.Id
        });
    }
}