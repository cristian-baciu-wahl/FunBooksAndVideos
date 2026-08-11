using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Mappers;

public static class PurchaseOrderMapper
{
    public static PurchaseOrderResponse ToResponse(
        this PurchaseOrder order)
    {
        return new PurchaseOrderResponse
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            ItemLines = [.. order.ItemLines.Select(item => new PurchaseOrderItemResponse
                {
                    Id = item.Id,
                    ProductName = item.Product?.Name,
                    MembershipType = item.MembershipType,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                })],
            TotalPrice = order.TotalPrice
        };
    }
}
