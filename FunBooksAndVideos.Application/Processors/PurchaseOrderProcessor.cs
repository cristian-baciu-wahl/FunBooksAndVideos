using FunBooksAndVideos.Application.Exceptions;
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

    public async Task<PurchaseOrder> ProcessPurchaseOrderAsync(PurchaseOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = new PurchaseOrder(request.Id, request.CustomerId);

        foreach (var item in request.Items)
        {
            var itemLine = new ItemLine
            {
                Quantity = item.Quantity,
            };

            if (item.ProductId.HasValue)
            {
                var productId = item.ProductId.Value;

                // A better design would be to use ProblemDetails.
                var product = await repository.GetProductByIdAsync(productId) ?? throw new ProductNotFoundException(productId);

                itemLine.Product = product;

                itemLine.UnitPrice = product.Price; 
            }

            if (!string.IsNullOrWhiteSpace(item.MembershipType) &&
                Enum.TryParse<MembershipType>(item.MembershipType, true, out var membershipType))
            {
                itemLine.MembershipType = membershipType;
                itemLine.UnitPrice = 0m; //I am not sure membership should have a price
            }

            order.ItemLines.Add(itemLine);
        }

        _ruleEngine.ExecuteRules(order);

        await repository.SavePurchaseOrderAsync(order);

        return order;
    }
}

