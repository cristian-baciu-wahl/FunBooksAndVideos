using FunBooksAndVideos.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.Property(video => video.Director)
           .HasMaxLength(200)
           .IsRequired();

        builder.Property(video => video.DurationInMinutes)
            .IsRequired();

        builder.HasData(
            new Video
            {
                Id = 2,
                Name = "Comprehensive First Aid Training",
                Director = "John Smith",
                DurationInMinutes = 0,
                Price = 33.51m
            });
    }
}