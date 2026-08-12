using FunBooksAndVideos.Domain.Customers;

namespace FunBooksAndVideos.Application.PurchaseOrders.Contracts;

public sealed class PurchaseOrderItemResponse
{
    public int Id { get; internal set; }
    public int Quantity { get; internal set; }
    public decimal UnitPrice { get; internal set; } = decimal.Zero;
    public string? ProductName { get; internal set; }
    public MembershipType? MembershipType { get; internal set; }
    public decimal TotalPrice { get; internal set; }
}

