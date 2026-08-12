using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Domain.Catalog;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.BusinessRules;

/// <summary>
/// BR2: If the purchase order contains a physical product,
/// a shipping slip has to be generated.
/// </summary>
public sealed class GenerateShippingSlipRule(
    IShippingSlipService service) : IBusinessRule
{
    public string RuleId => "BR2_GenerateShippingSlip";

    public int Priority => 20;

    public RuleExecutionStage Stage => RuleExecutionStage.PostProcessing;

    public bool ShouldApply(PurchaseOrder order)
    {
        return order.ItemLines
            .OfType<ProductOrderLine>()
            .Any(line => line.Product.Type == ProductType.Physical);
    }

    public async Task ApplyAsync(
        PurchaseOrder order,
        CancellationToken cancellationToken = default)
    {
        await service.GenerateShippingSlipAsync(
            order.Id,
            order.CustomerId,
            cancellationToken);
    }
}