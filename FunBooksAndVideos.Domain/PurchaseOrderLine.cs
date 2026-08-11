namespace FunBooksAndVideos.Domain;

public abstract class PurchaseOrderLine
{
    public int Id { get; protected set; }
    public decimal UnitPrice { get; protected set; }

    public abstract decimal TotalPrice { get; }
}
