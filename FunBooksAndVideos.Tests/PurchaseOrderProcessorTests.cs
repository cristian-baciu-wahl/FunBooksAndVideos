using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Exceptions;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Application.Rules;
using FunBooksAndVideos.Application.Services;
using FunBooksAndVideos.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace FunBooksAndVideos.Tests;

public class PurchaseOrderProcessorTests
{
    private readonly Mock<IShippingSlipService> _shipping = new();
    private readonly Mock<ICustomerMembershipService> _membership = new();
    private readonly Mock<IPurchaseOrderRepository> _repo = new();
    private readonly PurchaseOrderProcessor _sut;

    public PurchaseOrderProcessorTests()
    {
        // We added all our rules here, but depending on how we test, we might decide to test rules in isolation 
        var engine = new BusinessRuleEngine(Mock.Of<ILogger<BusinessRuleEngine>>(), []);
        engine.AddRule(new ActivateMembershipRule(_membership.Object));
        engine.AddRule(new GenerateShippingSlipRule(_shipping.Object));
        _sut = new PurchaseOrderProcessor(engine, _repo.Object);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WhenProductDoesNotExist_DoesNotSaveOrRunRules()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 1,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 999,
                    Quantity = 1
                }
            ]
        };

        _repo.Setup(x => x.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(() => _sut.ProcessPurchaseOrderAsync(request));

        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Never);

        _membership.Verify(
            x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()),
            Times.Never);

        _shipping.Verify(
            x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WhenProductIsDigital_DoesNotGenerateShippingSlip()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 2,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 2,
                    Quantity = 1
                }
            ]
        };

        _repo.Setup(x => x.GetProductByIdAsync(2))
            .ReturnsAsync(new Video
            {
                Id = 2,
                Name = "Comprehensive First Aid Training",
                Director = "John Smith",
                Price = 33.51m
            });

        var order = await _sut.ProcessPurchaseOrderAsync(request);

        Assert.Single(order.ItemLines);
        Assert.IsType<Video>((order.ItemLines[0] as ProductOrderLine)?.Product);

        _shipping.Verify(
            x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);

        _membership.Verify(
            x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()),
            Times.Never);

        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithBookMembership_ActivatesMembershipOnly()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 10,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    MembershipType = MembershipType.BookClub.ToString(),
                    Quantity = 1
                }
            ]
        };

        var order = await _sut.ProcessPurchaseOrderAsync(request);

        Assert.Single(order.ItemLines);
        Assert.Equal(0m, order.TotalPrice);

        _membership.Verify(
            x => x.ActivateMembership(4567890, MembershipType.BookClub),
            Times.Once);

        _shipping.Verify(
            x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);

        _repo.Verify(x => x.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithPhysicalBook_GeneratesOneShippingSlip()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 11,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            ]
        };

        _repo.Setup(x => x.GetProductByIdAsync(1))
            .ReturnsAsync(new Book
            {
                Id = 1,
                Name = "The Girl on the Train",
                Author = "Paula Hawkins",
                Isbn = "9781234567897",
                Price = 14.99m
            });

        var order = await _sut.ProcessPurchaseOrderAsync(request);

        Assert.Equal(29.98m, order.TotalPrice);

        _shipping.Verify(
            x => x.GenerateShippingSlip(11, 4567890),
            Times.Once);
        _membership.Verify(
            x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()),
            Times.Never);
        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithMultiplePhysicalItems_GeneratesSingleShippingSlip()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 12,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest { ProductId = 1, Quantity = 1 },
                new PurchaseOrderItemRequest { ProductId = 1, Quantity = 3 }
            ]
        };

        _repo.Setup(x => x.GetProductByIdAsync(1))
            .ReturnsAsync(new Book
            {
                Id = 1,
                Name = "The Girl on the Train",
                Author = "Paula Hawkins",
                Isbn = "9781234567897",
                Price = 14.99m
            });

        var order = await _sut.ProcessPurchaseOrderAsync(request);

        Assert.Equal(59.96m, order.TotalPrice);

        _shipping.Verify(
            x => x.GenerateShippingSlip(12, 4567890),
            Times.Once);

        _repo.Verify(x => x.GetProductByIdAsync(1), Times.Exactly(2));

        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public void ActivateMembership_WithPremium_ActivatesPremiumBookAndVideoClubs()
    {
        var sut = new CustomerMembershipService();

        sut.ActivateMembership(4567890, MembershipType.Premium);

        Assert.True(sut.HasActiveMembership(4567890, MembershipType.Premium));
        Assert.True(sut.HasActiveMembership(4567890, MembershipType.BookClub));
        Assert.True(sut.HasActiveMembership(4567890, MembershipType.VideoClub));
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.ProcessPurchaseOrderAsync(null!));
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_UsesCataloguePrice_NotClientSuppliedPrice()
    {
        var request = new PurchaseOrderRequest
        {
            Id = 20,
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            ]
        };

        _repo.Setup(x => x.GetProductByIdAsync(1))
            .ReturnsAsync(new Book
            {
                Id = 1,
                Name = "The Girl on the Train",
                Author = "Paula Hawkins",
                Isbn = "9781234567897",
                Price = 14.99m
            });

        var order = await _sut.ProcessPurchaseOrderAsync(request);

        Assert.Equal(14.99m, order.ItemLines[0].UnitPrice);
        Assert.Equal(29.98m, order.TotalPrice);
    }
}