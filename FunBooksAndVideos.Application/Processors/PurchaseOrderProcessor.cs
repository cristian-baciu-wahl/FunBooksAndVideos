using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Processors;

/// <summary>
/// Purchase Order Processor using Strategy Pattern for business rules
/// </summary>
public class PurchaseOrderProcessor(IBusinessRuleEngine ruleEngine, IPurchaseOrderRepository repository) : IPurchaseOrderProcessor
{
    private readonly IBusinessRuleEngine _ruleEngine = ruleEngine;

    public async Task<PurchaseOrder> ProcessPurchaseOrderAsync(CreatePurchaseOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = new PurchaseOrder(request.Id, request.CustomerId);

        foreach (var item in request.Items)
        {
            var itemLine = new ItemLine
            {
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

            // Only look up a product when a ProductId was supplied
            if (item.ProductId.HasValue)
            {
                var product = await repository.GetProductByIdAsync(item.ProductId.Value);
                itemLine.Product = product;
            }

            // Only set membership when a valid membership type string is provided
            if (!string.IsNullOrWhiteSpace(item.MembershipType) &&
                Enum.TryParse<MembershipType>(item.MembershipType, true, out var membershipType))
            {
                itemLine.MembershipType = membershipType;
            }

            order.ItemLines.Add(itemLine);
        }

        _ruleEngine.ExecuteRules(order);

        await repository.SavePurchaseOrderAsync(order);

        return order;
    }
}

