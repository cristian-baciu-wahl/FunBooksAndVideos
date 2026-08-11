namespace FunBooksAndVideos.Application.Models;

public class PurchaseOrderRequest
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public List<PurchaseOrderItemRequest> Items { get; set; } = [];
}
