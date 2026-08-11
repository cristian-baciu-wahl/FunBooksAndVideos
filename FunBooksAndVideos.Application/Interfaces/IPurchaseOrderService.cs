using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
}
