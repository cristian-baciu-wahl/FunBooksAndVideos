namespace FunBooksAndVideos.Domain.Catalog;
public class Video : Product
{
    public required string Director { get; set; }
    public int DurationInMinutes { get; set; }
    public override ProductType Type => ProductType.Digital;
}
