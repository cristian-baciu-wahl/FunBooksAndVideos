namespace FunBooksAndVideos.Application.Models;

public class PurchaseOrderItemRequest
{
    public int? ProductId { get; set; }

    public string? MembershipType { get; set; }

    public int Quantity { get; set; } = 1;
}