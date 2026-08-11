using FunBooksAndVideos.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public sealed class MembershipOrderLineConfiguration
    : IEntityTypeConfiguration<MembershipOrderLine>
{
    public void Configure(EntityTypeBuilder<MembershipOrderLine> builder)
    {
        builder.Property(line => line.MembershipType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}