namespace FunBooksAndVideos.Infrastructure.Fullfilment;

public interface IShippingSlipPublisher
{
    Task PublishAsync(
        int purchaseOrderId,
        int customerId,
        CancellationToken cancellationToken = default);
}