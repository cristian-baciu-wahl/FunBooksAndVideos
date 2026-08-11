namespace FunBooksAndVideos.Domain;

public class Customer(int id, string name, string email)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public bool IsActive { get; set; } = true;
    public List<Membership> Memberships { get; set; } = [];
}
