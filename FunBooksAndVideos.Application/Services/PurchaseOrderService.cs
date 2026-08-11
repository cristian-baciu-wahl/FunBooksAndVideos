using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Services;

public class PurchaseOrderService(IPurchaseOrderRepository repository) : IPurchaseOrderService
{
    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
    {
        return await repository.GetPurchaseOrderByIdAsync(id);
    }
}
