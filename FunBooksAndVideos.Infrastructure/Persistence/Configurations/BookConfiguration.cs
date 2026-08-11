using FunBooksAndVideos.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(book => book.Author)
           .HasMaxLength(200)
           .IsRequired();

        builder.Property(book => book.Isbn)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasData(
            new Book
            {
                Id = 1,
                Name = "The Girl on the train",
                Author = "Paula Hawkins",
                Isbn = "9781234567897",
                Price = 14.99m
            });
    }
}