namespace FunBooksAndVideos.Domain;

public class ItemLine
{
    public int Id { get; set; }
    public Product? Product { get; set; }
    public MembershipType? MembershipType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public bool IsMembership => MembershipType.HasValue;
    public bool IsPhysicalProduct => Product != null && Product.Type == ProductType.Physical;
}
