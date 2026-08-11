using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder> SavePurchaseOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
    Task<Product?> GetProductByIdAsync(int id);
    bool ProductExists(int id);
}