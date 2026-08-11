namespace FunBooksAndVideos.Domain;

public sealed class MembershipOrderLine : PurchaseOrderLine
{
    public MembershipType MembershipType { get; private set; }

    public override decimal TotalPrice => UnitPrice;

    private MembershipOrderLine() { }

    public MembershipOrderLine(MembershipType membershipType, decimal price = 0m)
    {
        MembershipType = membershipType;
        UnitPrice = price;
    }
}