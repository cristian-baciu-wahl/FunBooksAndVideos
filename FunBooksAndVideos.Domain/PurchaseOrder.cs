namespace FunBooksAndVideos.Domain;

public class PurchaseOrder(int id, int customerId)
{
    public int Id { get; set; } = id;
    public int CustomerId { get; set; } = customerId;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public List<ItemLine> ItemLines { get; set; } = [];
    public decimal TotalPrice => ItemLines.Sum(item => item.UnitPrice * item.Quantity);
}
