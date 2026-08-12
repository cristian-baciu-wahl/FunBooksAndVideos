namespace FunBooksAndVideos.Application.Interfaces;

public interface IShippingSlipPublisher
{
    Task PublishAsync(
        int purchaseOrderId,
        int customerId,
        CancellationToken cancellationToken = default);
}