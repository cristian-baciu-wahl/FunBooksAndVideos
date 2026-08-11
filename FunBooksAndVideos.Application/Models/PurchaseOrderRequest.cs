namespace FunBooksAndVideos.Application.Models;

public class PurchaseOrderRequest
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public List<OrderItemRequest> Items { get; set; } = [];
}


// The order item is always part of a create purchase order request
// For simplicity we can keep these 2 together as long as they are small
public class OrderItemRequest
{
    public int? ProductId { get; set; }

    public string? MembershipType { get; set; }

    public int Quantity { get; set; } = 1;

    public decimal UnitPrice { get; set; }
}