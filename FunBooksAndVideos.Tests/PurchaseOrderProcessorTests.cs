using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Application.Rules;
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
        var engine = new BusinessRuleEngine(Mock.Of<ILogger<BusinessRuleEngine>>());
        engine.AddRule(new ActivateMembershipRule(_membership.Object));
        engine.AddRule(new GenerateShippingSlipRule(_shipping.Object));
        _sut = new PurchaseOrderProcessor(engine, _repo.Object);
    }

    [Theory]
    [MemberData(nameof(TestScenarios))]
    public async Task ProcessPurchaseOrderAsync_AppliesRules(CreatePurchaseOrderRequest request, bool expectMembership, bool expectShipping)
    {
        // Arrange
        SetupRepo(request);

        // Act
        var result = await _sut.ProcessPurchaseOrderAsync(request);

        // Assert
        Assert.Equal(request.Items.Count, result.ItemLines.Count);
        _membership.Verify(x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()),
            expectMembership ? Times.AtLeastOnce() : Times.Never());
        _shipping.Verify(x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()),
            expectShipping ? Times.AtLeastOnce() : Times.Never());
        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_NullRequest_Throws() =>
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ProcessPurchaseOrderAsync(null));

    [Fact]
    public async Task ProcessPurchaseOrderAsync_DoesNotCallGetProduct_WhenItemsAreOnlyMemberships()
    {
        // Arrange
        var request = CreateRequest(MembershipType.BookClub, false);
        // No product ids present so repository GetProductByIdAsync should not be called

        // Act
        var result = await _sut.ProcessPurchaseOrderAsync(request);

        // Assert
        Assert.Single(result.ItemLines);
        _repo.Verify(x => x.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_CallsGetProduct_ForProductItems()
    {
        // Arrange
        var request = CreateRequest(null, true);
        SetupRepo(request);

        // Act
        var result = await _sut.ProcessPurchaseOrderAsync(request);

        // Assert
        Assert.Single(result.ItemLines);
        _repo.Verify(x => x.GetProductByIdAsync(It.IsAny<int>()), Times.Once);
        _repo.Verify(x => x.SavePurchaseOrderAsync(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    // helper for building requests moved to CreateRequest; synchronous PurchaseOrder builders removed

    private void SetupRepo(CreatePurchaseOrderRequest request)
    {
        foreach (var item in request.Items.Where(x => x.ProductId.HasValue))
            _repo.Setup(x => x.GetProductByIdAsync(item.ProductId.Value))
                .ReturnsAsync(new Book { Name = "Test", Isbn = "00000000", Author = "Allan Joe", Id = 1 });
    }

    public static TheoryData<CreatePurchaseOrderRequest, bool, bool> TestScenarios() => new()
    {
        { CreateRequest(MembershipType.BookClub, false), true, false },
        { CreateRequest(null, true), false, true },
        { CreateRequest(MembershipType.Premium, true), true, true },
        { CreateRequest(null, false), false, false }
    };

    private static CreatePurchaseOrderRequest CreateRequest(MembershipType? membership, bool hasPhysical)
    {
        var items = new List<OrderItemRequest>();
        if (membership.HasValue)
            items.Add(new OrderItemRequest { MembershipType = membership.ToString(), Quantity = 1 });
        if (hasPhysical)
            items.Add(new OrderItemRequest { ProductId = 1, Quantity = 1, UnitPrice = 14.99m });
        return new CreatePurchaseOrderRequest { Id = 1, CustomerId = 12345, Items = items };
    }
}