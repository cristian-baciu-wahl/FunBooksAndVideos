using FunBooksAndVideos.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations.Customers;

public sealed class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Memberships");

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.Id)
            .ValueGeneratedOnAdd();

        builder.Property(membership => membership.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(membership => membership.ActivationDate)
            .IsRequired();

        builder.Property(membership => membership.IsActive)
            .IsRequired();

        builder.Property(membership => membership.CustomerId)
            .IsRequired();

        // A customer has at most one record of each membership type.
        builder.HasIndex(membership => new
        {
            membership.CustomerId,
            membership.Type
        })
            .IsUnique();
    }
}