namespace FunBooksAndVideos.Domain.Fullfilment;

public sealed class ShippingSlip
{
    public int Id { get; private set; }

    public int PurchaseOrderId { get; private set; }

    public int CustomerId { get; private set; }

    public DateTime GeneratedAt { get; private set; } = DateTime.UtcNow;

    private ShippingSlip()
    {
    }

    public ShippingSlip(int purchaseOrderId, int customerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(purchaseOrderId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);

        PurchaseOrderId = purchaseOrderId;
        CustomerId = customerId;
    }
}