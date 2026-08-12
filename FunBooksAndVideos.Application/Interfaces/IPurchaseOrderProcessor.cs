using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface IPurchaseOrderProcessor
{
    Task<PurchaseOrder> ProcessPurchaseOrderAsync(PurchaseOrderRequest request, CancellationToken cancellationToken);
}