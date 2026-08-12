namespace FunBooksAndVideos.Application.PurchaseOrders.Create;

public class CreatePurchaseOrderRequest
{
    public int CustomerId { get; set; }

    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = [];
}
