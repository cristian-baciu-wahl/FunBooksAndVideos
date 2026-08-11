using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Processors;

/// <summary>
/// Purchase Order Processor using Strategy Pattern for business rules
/// </summary>
public class PurchaseOrderProcessor(IBusinessRuleEngine ruleEngine) : IPurchaseOrderProcessor
{
    private readonly IBusinessRuleEngine _ruleEngine = ruleEngine;

    public void ProcessPurchaseOrder(PurchaseOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        // Execute all applicable business rules via the rule engine
        _ruleEngine.ExecuteRules(order);
    }
}

