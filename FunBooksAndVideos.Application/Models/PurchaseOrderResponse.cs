namespace FunBooksAndVideos.Application.Models;

public sealed class PurchaseOrderResponse
{
    public int Id { get; internal set; }

    public int CustomerId { get; internal set; }

    public DateTime OrderDate { get; internal set; }

    public List<PurchaseOrderItemResponse> ItemLines { get; internal set; } = [];

    public decimal TotalPrice { get; internal set; }
}

