using FunBooksAndVideos.Domain.Fullfilment;

namespace FunBooksAndVideos.Infrastructure.Fullfilment;

public sealed class InMemoryShippingSlipPublisher : IShippingSlipPublisher
{
    private readonly List<ShippingSlip> _shippingSlips = [];

    // For testing purposes, we expose the published slips as a read-only collection.
    public IReadOnlyCollection<ShippingSlip> PublishedSlips =>
       _shippingSlips.AsReadOnly();

    public Task PublishAsync(
        int purchaseOrderId,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _shippingSlips.Add(
            new ShippingSlip(
                purchaseOrderId,
                customerId));

        return Task.CompletedTask;
    }

    public ShippingSlip? GetByPurchaseOrderId(int purchaseOrderId)
    {
        return _shippingSlips
            .SingleOrDefault(x => x.PurchaseOrderId == purchaseOrderId);
    }
}