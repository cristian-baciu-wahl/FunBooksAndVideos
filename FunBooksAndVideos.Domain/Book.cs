namespace FunBooksAndVideos.Domain;

public class Book : Product
{
    public required string Author { get; set; }
    public required string Isbn { get; set; }
    public override ProductType Type => ProductType.Physical;
}
