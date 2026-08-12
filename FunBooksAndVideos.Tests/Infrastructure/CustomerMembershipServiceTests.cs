using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Infrastructure.Persistence;
using FunBooksAndVideos.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Tests.Infrastructure;

public class CustomerMembershipServiceTests
{
    private const int CustomerId = 4567890;

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new AppDbContext(options);

        dbContext.Customers.Add(
            new Customer(
                CustomerId,
                "John Doe",
                "john.doe@example.com"));

        dbContext.SaveChanges();

        return dbContext;
    }

    [Fact]
    public void ActivateMembership_WithPremium_AddsAllRequiredMemberships()
    {
        using var dbContext = CreateDbContext();

        var sut = new CustomerMembershipService(dbContext);

        sut.ActivateMembership(
            4567890,
            MembershipType.Premium);

        var customer = dbContext.Customers
            .Include(x => x.Memberships)
            .Single(x => x.Id == 4567890);

        Assert.Equal(3, customer.Memberships.Count);

        Assert.Contains(
            customer.Memberships,
            x => x.Type == MembershipType.Premium && x.IsActive);

        Assert.Contains(
            customer.Memberships,
            x => x.Type == MembershipType.BookClub && x.IsActive);

        Assert.Contains(
            customer.Memberships,
            x => x.Type == MembershipType.VideoClub && x.IsActive);
    }

    [Fact]
    public void ActivateMembership_WithBookClub_AddsBookClubMembership()
    {
        using var dbContext = CreateDbContext();

        var sut = new CustomerMembershipService(dbContext);

        sut.ActivateMembership(
            4567890,
            MembershipType.BookClub);

        var customer = dbContext.Customers
            .Include(x => x.Memberships)
            .Single(x => x.Id == 4567890);

        Assert.Single(customer.Memberships);

        var membership = customer.Memberships.Single();

        Assert.Equal(MembershipType.BookClub, membership.Type);
        Assert.True(membership.IsActive);
        Assert.Equal(4567890, membership.CustomerId);
    }

    [Fact]
    public void ActivateMembership_WithExistingInactiveMembership_ReactivatesIt()
    {
        using var dbContext = CreateDbContext();

        var customer = dbContext.Customers
            .Single(x => x.Id == 4567890);

        customer.Memberships.Add(
            new Membership(MembershipType.BookClub)
            {
                CustomerId = 4567890,
                IsActive = false
            });

        var sut = new CustomerMembershipService(dbContext);

        sut.ActivateMembership(
            4567890,
            MembershipType.BookClub);

        var membership = customer.Memberships.Single();

        Assert.True(membership.IsActive);
    }
}