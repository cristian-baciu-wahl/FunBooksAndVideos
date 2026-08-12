using FunBooksAndVideos.Application.PurchaseOrders.Create;
using FunBooksAndVideos.Domain.Customers;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FunBooksAndVideos.Tests.PurchaseOrders;

// Info: this is a better test in my opinion, because it ressembles an integration test 
// Instead of focusing on testing implementation details, we test the end result for a given scenario.

// Info: I've added a happy case here too, but most of them are in the Postman Collection
// Preferably any E2E tests should be in Postman or other external tool, but they can developed with xUnit as well.

public class PurchaseOrderApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PurchaseOrderApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithUnknownProduct_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new CreatePurchaseOrderItemRequest
                {
                    ProductId = 999,
                    Quantity = 1,
                }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithProductAndMembershipOnSameLine_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new CreatePurchaseOrderItemRequest
                {
                    ProductId = 1,
                    MembershipType = MembershipType.BookClub.ToString(),
                    Quantity = 1
                }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithNeitherProductNorMembership_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new CreatePurchaseOrderItemRequest
                {
                    Quantity = 1
                }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithBlankMembershipType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new CreatePurchaseOrderItemRequest
                {
                    MembershipType = " ",
                    Quantity = 1
                }
            ]
        };

        // Act  
        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
   
    [Fact]
    public async Task CreatePurchaseOrder_WithBookMembership_ReturnsCreated()
    {
        // Arrange
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new CreatePurchaseOrderItemRequest
                {
                    MembershipType = MembershipType.BookClub.ToString(),
                    Quantity = 1
                }
            ]
        };

        // Act  
        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}