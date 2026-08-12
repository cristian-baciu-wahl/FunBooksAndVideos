namespace FunBooksAndVideos.Domain.Customers;

public class Membership(MembershipType type)
{
    public int Id { get; set; }
    public MembershipType Type { get; set; } = type;
    public DateTime ActivationDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public int CustomerId { get; set; }
}
