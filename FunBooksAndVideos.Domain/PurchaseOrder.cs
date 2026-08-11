
//another way of representing namespaces
using FunBooksAndVideos.Domain;

public sealed class PurchaseOrder(int id, int customerId)
{
    public int Id { get; private set; } = id;
    public int CustomerId { get; private set; } = customerId;
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

    public List<PurchaseOrderLine> ItemLines { get; private set; } = [];

    public decimal TotalPrice => ItemLines.Sum(line => line.TotalPrice);
}
