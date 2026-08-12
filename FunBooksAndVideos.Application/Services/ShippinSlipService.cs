using FunBooksAndVideos.Application.Interfaces;

namespace FunBooksAndVideos.Application.Services;

public sealed class ShippingSlipService(IShippingSlipPublisher publisher) : IShippingSlipService
{
    public async Task GenerateShippingSlip(
        int purchaseOrderId,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        await publisher.PublishAsync(purchaseOrderId, customerId, cancellationToken);
    }
}