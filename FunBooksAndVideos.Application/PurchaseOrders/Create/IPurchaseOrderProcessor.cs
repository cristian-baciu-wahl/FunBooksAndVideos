using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.PurchaseOrders.Create;

public interface IPurchaseOrderProcessor
{
    Task<PurchaseOrder> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken);
}