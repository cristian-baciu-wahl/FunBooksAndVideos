namespace FunBooksAndVideos.Application.Interfaces;

public interface IShippingSlipService
{
    Task GenerateShippingSlip(int purchaseOrderId, int customerId, CancellationToken cancellationToken);
}