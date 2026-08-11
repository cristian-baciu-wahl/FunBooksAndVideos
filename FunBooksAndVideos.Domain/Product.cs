namespace FunBooksAndVideos.Domain;

/// <summary>
/// This class is used as a base for all specific products 
/// </summary>
public abstract class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public abstract ProductType Type { get; }
}
