using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Infrastructure.Repositories;

public sealed class EfPurchaseOrderRepository(AppDbContext dbContext)
    : IPurchaseOrderRepository
{
    public async Task<PurchaseOrder> SavePurchaseOrderAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        await dbContext.PurchaseOrders.AddAsync(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.PurchaseOrders
            .Include(purchaseOrder => purchaseOrder.ItemLines)
            .SingleOrDefaultAsync(purchaseOrder => purchaseOrder.Id == id, cancellationToken);

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
            await dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .LoadAsync(cancellationToken);
        }

        return order;
    }

    public Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);

    public bool ProductExists(int id) =>
        dbContext.Products.Any(product => product.Id == id);
}
