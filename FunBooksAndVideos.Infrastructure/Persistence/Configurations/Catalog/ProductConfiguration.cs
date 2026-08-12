using FunBooksAndVideos.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .ValueGeneratedNever();

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        // Domain-calculated product type; EF uses ProductKind instead.
        builder.Ignore(product => product.Type);

        builder.HasDiscriminator<string>("ProductKind")
            .HasValue<Book>("Book")
            .HasValue<Video>("Video");
    }
}