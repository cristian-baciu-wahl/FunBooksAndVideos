namespace FunBooksAndVideos.Application.PurchaseOrders.Create;

public class CreatePurchaseOrderItemRequest
{
    public int? ProductId { get; set; }

    public string? MembershipType { get; set; }

    public int Quantity { get; set; } = 1;
}