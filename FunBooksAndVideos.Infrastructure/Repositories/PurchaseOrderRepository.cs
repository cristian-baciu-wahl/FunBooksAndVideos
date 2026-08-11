using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace FunBooksAndVideos.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ConcurrentDictionary<int, PurchaseOrder> _orders = new();
    private readonly ConcurrentDictionary<int, Product> _products = new();

    public PurchaseOrderRepository()
    {
        // Products can be loaded from a database and we could use EF to map our data
        InitializeSampleProducts();
    }

    private void InitializeSampleProducts()
    {
        _products.TryAdd(1, new Book
        {
            Id = 1,
            Name = "The Girl on the train",
            Author = "Paula Hawkins",
            Isbn = "9781234567897",
            Price = 14.99m
        });
        _products.TryAdd(2, new Video
        {
            Id = 2,
            Name = "Comprehensive First Aid Training",
            Director = "John Smith",
            Price = 33.51m
        });
    }

    public async Task SavePurchaseOrderAsync(PurchaseOrder order)
    {
        _orders.TryAdd(order.Id, order);
        await Task.CompletedTask;
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
    {
        _orders.TryGetValue(id, out var order);
        return await Task.FromResult(order);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        _products.TryGetValue(id, out var product);
        return await Task.FromResult(product);
    }

    public bool ProductExists(int id)
    {
        return _products.ContainsKey(id);
    }
}