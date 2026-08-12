using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Infrastructure.Fullfilment;

namespace FunBooksAndVideos.Infrastructure.Persistence.Services;

public sealed class ShippingSlipService(IShippingSlipPublisher publisher) : IShippingSlipService
{
    public async Task GenerateShippingSlipAsync(
        int purchaseOrderId,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        await publisher.PublishAsync(purchaseOrderId, customerId, cancellationToken);
    }
}