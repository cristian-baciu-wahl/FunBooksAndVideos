using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Rules;
using FunBooksAndVideos.Domain;
using Moq;

namespace FunBooksAndVideos.Tests.Rules;

public class ActivateMembershipRuleTests
{
    private const int CustomerId = 4567890;

    private readonly Mock<ICustomerMembershipService> _membership = new();

    private readonly ActivateMembershipRule _sut;

    public ActivateMembershipRuleTests()
    {
        _sut = new ActivateMembershipRule(_membership.Object);
    }

    [Fact]
    public void ShouldApply_WhenOrderContainsMembership_ReturnsTrue()
    {
        var order = CreateMembershipOrder(
            MembershipType.BookClub);

        Assert.True(_sut.ShouldApply(order));
    }

    [Fact]
    public void ShouldApply_WhenOrderContainsProduct_ReturnsFalse()
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new ProductOrderLine(CreateBook(), 1));

        Assert.False(_sut.ShouldApply(order));
    }

    [Fact]
    public void Apply_WithBookClub_ActivatesBookClub()
    {
        var order = CreateMembershipOrder(
            MembershipType.BookClub);

        _sut.Apply(order);

        _membership.Verify(
            x => x.ActivateMembership(
                CustomerId,
                MembershipType.BookClub),
            Times.Once);
    }

    [Fact]
    public void Apply_WithVideoClub_ActivatesVideoClub()
    {
        var order = CreateMembershipOrder(
            MembershipType.VideoClub);

        _sut.Apply(order);

        _membership.Verify(
            x => x.ActivateMembership(
                CustomerId,
                MembershipType.VideoClub),
            Times.Once);
    }

    [Fact]
    public void Apply_WithPremium_ActivatesPremium()
    {
        var order = CreateMembershipOrder(
            MembershipType.Premium);

        _sut.Apply(order);

        _membership.Verify(
            x => x.ActivateMembership(
                CustomerId,
                MembershipType.Premium),
            Times.Once);
    }

    [Fact]
    public void Apply_WithMultipleMemberships_ActivatesEachMembership()
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new MembershipOrderLine(MembershipType.BookClub));

        order.ItemLines.Add(
            new MembershipOrderLine(MembershipType.VideoClub));

        _sut.Apply(order);

        _membership.Verify(
            x => x.ActivateMembership(
                CustomerId,
                MembershipType.BookClub),
            Times.Once);

        _membership.Verify(
            x => x.ActivateMembership(
                CustomerId,
                MembershipType.VideoClub),
            Times.Once);
    }

    private static PurchaseOrder CreateMembershipOrder(
        MembershipType membershipType)
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new MembershipOrderLine(membershipType));

        return order;
    }

    private static Book CreateBook() =>
        new()
        {
            Id = 1,
            Name = "The Girl on the Train",
            Author = "Paula Hawkins",
            Isbn = "9781234567897",
            Price = 14.99m
        };
}