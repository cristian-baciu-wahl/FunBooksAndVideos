using FunBooksAndVideos.Application.BusinessRules;
using Moq;
using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Domain.Catalog;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Tests.BusinessRules;

public class GenerateShippingSlipRuleTests
{
    private const int CustomerId = 4567890;

    private readonly Mock<IShippingSlipService> _shippingService = new();

    private readonly GenerateShippingSlipRule _sut;

    public GenerateShippingSlipRuleTests()
    {
        _sut = new GenerateShippingSlipRule(_shippingService.Object);
    }

    [Fact]
    public void ShouldApply_WhenOrderContainsPhysicalProduct_ReturnsTrue()
    {
        var order = CreateOrderWithBook();

        Assert.True(_sut.ShouldApply(order));
    }

    [Fact]
    public void ShouldApply_WhenOrderContainsDigitalProduct_ReturnsFalse()
    {
        var order = CreateOrderWithVideo();

        Assert.False(_sut.ShouldApply(order));
    }

    [Fact]
    public void ShouldApply_WhenOrderContainsOnlyMembership_ReturnsFalse()
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new MembershipOrderLine(MembershipType.BookClub));

        Assert.False(_sut.ShouldApply(order));
    }

    [Fact]
    public async Task Apply_PassesOrderIdAndCustomerIdToShippingService()
    {
        var order = CreateOrderWithBook();

        await _sut.ApplyAsync(order, CancellationToken.None);

        _shippingService.Verify(
            x => x.GenerateShippingSlipAsync(
                order.Id,
                CustomerId,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Apply_WithMultiplePhysicalItems_GeneratesOneShippingSlip()
    {
        var order = new PurchaseOrder(CustomerId);

        var book = CreateBook();

        order.ItemLines.Add(new ProductOrderLine(book, 1));
        order.ItemLines.Add(new ProductOrderLine(book, 3));

        await _sut.ApplyAsync(order, CancellationToken.None);

        _shippingService.Verify(
            x => x.GenerateShippingSlipAsync(
                order.Id,
                CustomerId,
                CancellationToken.None),
            Times.Once);
    }

    private static PurchaseOrder CreateOrderWithBook()
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new ProductOrderLine(CreateBook(), 1));

        return order;
    }

    private static PurchaseOrder CreateOrderWithVideo()
    {
        var order = new PurchaseOrder(CustomerId);

        order.ItemLines.Add(
            new ProductOrderLine(CreateVideo(), 1));

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

    private static Video CreateVideo() =>
        new()
        {
            Id = 2,
            Name = "Comprehensive First Aid Training",
            Director = "John Smith",
            Price = 33.51m
        };
}