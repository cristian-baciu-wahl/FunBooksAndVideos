using FunBooksAndVideos.Application.BusinessRules;
using FunBooksAndVideos.Application.Exceptions;
using FunBooksAndVideos.Application.PurchaseOrders.Ports;
using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.PurchaseOrders.Create;

/// <summary>
/// Purchase Order Processor using Strategy Pattern for business rules
/// </summary>
public class PurchaseOrderProcessor(IBusinessRuleEngine ruleEngine, IPurchaseOrderRepository repository) : IPurchaseOrderProcessor
{
    private readonly IBusinessRuleEngine _ruleEngine = ruleEngine;

    public async Task<PurchaseOrder> CreatePurchaseOrderAsync(
        CreatePurchaseOrderRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = new PurchaseOrder(request.CustomerId);

        foreach (var item in request.Items)
        {
            if (item.ProductId.HasValue)
            {
                var product = await repository.GetProductByIdAsync(item.ProductId.Value, cancellationToken)
                    ?? throw new ProductNotFoundException(item.ProductId.Value);

                order.ItemLines.Add(new ProductOrderLine(product, item.Quantity));
            }
            else
            {
                var membershipType = Enum.Parse<MembershipType>(item.MembershipType!, true);

                order.ItemLines.Add(new MembershipOrderLine(membershipType));
            }
        }

        await _ruleEngine.ExecuteRulesAsync(order, RuleExecutionStage.PreProcessing, cancellationToken);

        await repository.SavePurchaseOrderAsync(order, cancellationToken);

        await _ruleEngine.ExecuteRulesAsync(order, RuleExecutionStage.PostProcessing, cancellationToken);

        return order;
    }
}

