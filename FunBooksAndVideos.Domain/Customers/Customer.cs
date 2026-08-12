namespace FunBooksAndVideos.Domain.Customers;
public class Customer(int id, string name, string email)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public bool IsActive { get; set; } = true;

    public List<Membership> Memberships { get; set; } = [];

    public void ActivateMembership(MembershipType membershipType)
    {
        if (!IsActive)
            throw new InvalidOperationException(
                $"Customer {Id} is not active.");

        switch (membershipType)
        {
            case MembershipType.Premium:
                Activate(MembershipType.BookClub);
                Activate(MembershipType.VideoClub);
                break;

            default:
                Activate(membershipType);
                break;
        }
    }

    private void Activate(MembershipType membershipType)
    {
        var existingMembership = Memberships
            .SingleOrDefault(membership => membership.Type == membershipType);

        if (existingMembership is not null)
        {
            existingMembership.IsActive = true;
            existingMembership.ActivationDate = DateTime.UtcNow;
            return;
        }

        Memberships.Add(new Membership(membershipType)
        {
            CustomerId = Id
        });
    }
}
