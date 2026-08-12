using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.PurchaseOrders.Services;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken);
}
