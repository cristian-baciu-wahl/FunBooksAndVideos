using FunBooksAndVideos.Application.PurchaseOrders.Ports;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.PurchaseOrders.Services;

public class PurchaseOrderService(IPurchaseOrderRepository repository) : IPurchaseOrderService
{
    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetPurchaseOrderByIdAsync(id, cancellationToken);
    }
}
