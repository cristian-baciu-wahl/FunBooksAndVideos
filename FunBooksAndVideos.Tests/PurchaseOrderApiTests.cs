using System.Net;
using System.Net.Http.Json;
using FunBooksAndVideos.Application.Models;
using Microsoft.AspNetCore.Mvc.Testing;

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
            Id = 9999,
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
}