namespace FunBooksAndVideos.Application.Interfaces;

public interface IShippingSlipService
{
    void GenerateShippingSlip(int purchaseOrderId, int customerId);
}