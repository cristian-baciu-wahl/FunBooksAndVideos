using FunBooksAndVideos.Domain.PurchaseOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations.PurchaseOrders;

public sealed class PurchaseOrderLineConfiguration
    : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines", table =>
            table.HasCheckConstraint(
                "CK_PurchaseOrderLines_ExactlyOneLineType",
                "([LineType] = 'Product' AND [ProductId] IS NOT NULL AND [MembershipType] IS NULL) " +
                "OR ([LineType] = 'Membership' AND [ProductId] IS NULL AND [MembershipType] IS NOT NULL)"));

        builder.HasKey(line => line.Id);

        builder.Property(line => line.Id)
            .ValueGeneratedOnAdd();

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        // Calculated by each concrete subtype.
        builder.Ignore(line => line.TotalPrice);

        builder.HasDiscriminator<string>("LineType")
            .HasValue<ProductOrderLine>("Product")
            .HasValue<MembershipOrderLine>("Membership");
    }
}