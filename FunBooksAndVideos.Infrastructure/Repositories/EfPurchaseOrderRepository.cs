using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Infrastructure.Repositories;

public sealed class EfPurchaseOrderRepository(AppDbContext dbContext)
    : IPurchaseOrderRepository
{
    public async Task SavePurchaseOrderAsync(PurchaseOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await dbContext.PurchaseOrders.AddAsync(order);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
    {
        var order = await dbContext.PurchaseOrders
            .Include(purchaseOrder => purchaseOrder.ItemLines)
            .SingleOrDefaultAsync(purchaseOrder => purchaseOrder.Id == id);

        if (order is null)
        {
            return null;
        }

        var productIds = order.ItemLines
            .OfType<ProductOrderLine>()
            .Select(line => line.ProductId)
            .Distinct()
            .ToList();

        if (productIds.Count > 0)
        {
            // Loading the products into this tracked context fixes up the Product
            // navigation on each ProductOrderLine for the response mapper.
            await dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .LoadAsync();
        }

        return order;
    }

    public Task<Product?> GetProductByIdAsync(int id) =>
        dbContext.Products.SingleOrDefaultAsync(product => product.Id == id);

    public bool ProductExists(int id) =>
        dbContext.Products.Any(product => product.Id == id);
}
