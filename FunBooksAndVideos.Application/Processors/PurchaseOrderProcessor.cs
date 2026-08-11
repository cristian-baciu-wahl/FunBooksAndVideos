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

        var order = new PurchaseOrder(request.CustomerId);

        foreach (var item in request.Items)
        {
            if (item.ProductId.HasValue)
            {
                var product = await repository.GetProductByIdAsync(item.ProductId.Value)
                    ?? throw new ProductNotFoundException(item.ProductId.Value);

                order.ItemLines.Add(new ProductOrderLine(product, item.Quantity));
            }
            else
            {
                var membershipType = Enum.Parse<MembershipType>(item.MembershipType!, true);

                order.ItemLines.Add(new MembershipOrderLine(membershipType));
            }
        }

        _ruleEngine.ExecuteRules(order);

        await repository.SavePurchaseOrderAsync(order);

        return order;
    }
}

