namespace FunBooksAndVideos.Application.BusinessRules.Ports;

public interface IShippingSlipService
{
    Task GenerateShippingSlipAsync(int purchaseOrderId, int customerId, CancellationToken cancellationToken);
}