using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Data.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.FileSize)
            .IsRequired();

        builder.Property(r => r.UploadDate)
            .IsRequired();
    }
}
