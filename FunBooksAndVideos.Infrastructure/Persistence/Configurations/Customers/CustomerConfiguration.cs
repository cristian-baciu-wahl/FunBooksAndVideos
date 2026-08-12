using FunBooksAndVideos.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .ValueGeneratedNever();

        builder.Property(customer => customer.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(customer => customer.IsActive)
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique();

        builder.HasMany(customer => customer.Memberships)
            .WithOne()
            .HasForeignKey(membership => membership.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Customer(4567890, "John Doe", "john.doe@example.com")
            {
                IsActive = true
            });
    }
}