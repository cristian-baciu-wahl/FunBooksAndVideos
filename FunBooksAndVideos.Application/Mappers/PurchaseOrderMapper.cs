using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Mappers;

public static class PurchaseOrderMapper
{
    public static PurchaseOrderResponse ToResponse(this PurchaseOrder order)
    {
        return new PurchaseOrderResponse
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            ItemLines = [.. order.ItemLines.Select(ToItemResponse)],
            TotalPrice = order.TotalPrice
        };
    }

    private static PurchaseOrderItemResponse ToItemResponse(PurchaseOrderLine line) =>
        line switch
        {
            ProductOrderLine productLine => MapProduct(productLine),
            MembershipOrderLine membershipLine => MapMembership(membershipLine),
            _ => throw new NotSupportedException(
                $"Unsupported purchase-order line type: {line.GetType().Name}")
        };

    private static PurchaseOrderItemResponse MapMembership(MembershipOrderLine membershipLine)
    {
        return new PurchaseOrderItemResponse
        {
            Id = membershipLine.Id,
            MembershipType = membershipLine.MembershipType,
            Quantity = 1,
            UnitPrice = membershipLine.UnitPrice,
            TotalPrice = membershipLine.TotalPrice
        };
    }

    private static PurchaseOrderItemResponse MapProduct(ProductOrderLine productLine)
    {
        return new PurchaseOrderItemResponse
        {
            Id = productLine.Id,
            ProductName = productLine.Product.Name,
            Quantity = productLine.Quantity,
            UnitPrice = productLine.UnitPrice,
            TotalPrice = productLine.TotalPrice
        };
    }
}