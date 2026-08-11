using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Rules;

/// <summary>
/// BR2: If the purchase order contains a physical product, a shipping slip has to be generated
/// </summary>
public class GenerateShippingSlipRule(IShippingSlipService shippingSlipService) : IBusinessRule
{
    private readonly IShippingSlipService _shippingSlipService = shippingSlipService;

    public string RuleId => "BR2_GenerateShippingSlip";

    // Lower priority - can be done after membership activation
    public int Priority => 20;

    public bool ShouldApply(PurchaseOrder order)
    {
        return order.ItemLines
            .OfType<ProductOrderLine>()
            .Any(line => line.Product.Type == ProductType.Physical);
    }

    public void Apply(PurchaseOrder order)
    {
        _shippingSlipService.GenerateShippingSlip(order.Id, order.CustomerId);
    }
}
