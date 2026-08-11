using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface IPurchaseOrderProcessor
{
    void ProcessPurchaseOrder(PurchaseOrder order);
}