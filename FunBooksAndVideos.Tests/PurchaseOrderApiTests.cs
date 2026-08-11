using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FunBooksAndVideos.Tests;

// Info: this is a better test in my opinion, because it ressembles an integration test 
// Instead of focusing on testing implementation details, we test the end result for a given scenario.

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
        var request = new PurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 999,
                    Quantity = 1,
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithProductAndMembershipOnSameLine_ReturnsBadRequest()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    MembershipType = MembershipType.BookClub.ToString(),
                    Quantity = 1
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithNeitherProductNorMembership_ReturnsBadRequest()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    Quantity = 1
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_WithBlankMembershipType_ReturnsBadRequest()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = 4567890,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    MembershipType = " ",
                    Quantity = 1
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/purchaseorder", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}