using FunBooksAndVideos.Domain.Catalog;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.PurchaseOrders.Ports;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder> SavePurchaseOrderAsync(PurchaseOrder order, CancellationToken cancellationToken);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    bool ProductExists(int id);
}