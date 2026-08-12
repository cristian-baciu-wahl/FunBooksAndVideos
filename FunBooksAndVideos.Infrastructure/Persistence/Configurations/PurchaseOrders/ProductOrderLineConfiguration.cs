using FunBooksAndVideos.Domain.PurchaseOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations.PurchaseOrders;

public sealed class ProductOrderLineConfiguration
    : IEntityTypeConfiguration<ProductOrderLine>
{
    public void Configure(EntityTypeBuilder<ProductOrderLine> builder)
    {
        builder.Property(line => line.ProductId)
            .IsRequired();

        builder.Property(line => line.Quantity)
            .IsRequired();

        builder.HasOne(line => line.Product)
            .WithMany()
            .HasForeignKey(line => line.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}