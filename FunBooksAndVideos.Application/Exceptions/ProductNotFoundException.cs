namespace FunBooksAndVideos.Application.Exceptions;

public sealed class ProductNotFoundException(int productId) 
    : Exception($"Product with ID {productId} was not found.")
{
    public int ProductId { get; } = productId;
}

