using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Infrastructure.Interfaces;

public interface IPurchaseOrderRepository
{
    Task SavePurchaseOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
    Task<Product?> GetProductByIdAsync(int id);
    bool ProductExists(int id);
}