namespace FunBooksAndVideos.Domain;

public sealed class ProductOrderLine : PurchaseOrderLine
{
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }

    public override decimal TotalPrice => UnitPrice * Quantity;

    // Parameterless constructor for EF Core
    private ProductOrderLine() { }

    public ProductOrderLine(Product product, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);

        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.Price;
    }
}

