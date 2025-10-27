using ECommerceBackend.Domain.Medias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;
internal sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("medias", "ecommerce-domain");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.OriginalFileName)
            .HasColumnName("original_file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.FilePath)
            .HasColumnName("file_path")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(m => m.FileUrl)
            .HasColumnName("file_url")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(m => m.FileSize)
            .HasColumnName("file_size")
            .IsRequired();

        builder.Property(m => m.MimeType)
            .HasColumnName("mime_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.MediaType)
            .HasColumnName("media_type")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(m => m.Width)
            .HasColumnName("width");

        builder.Property(m => m.Height)
            .HasColumnName("height");

        builder.Property(m => m.Duration)
            .HasColumnName("duration");

        builder.Property(m => m.UploadedBy)
            .HasColumnName("uploaded_by");

        builder.Property(m => m.IsTemp)
            .HasColumnName("is_temp")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.ConfirmedAtUtc)
            .HasColumnName("confirmed_at_utc");

        builder.Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(m => m.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        // Indexes
        builder.HasIndex(m => m.MediaType).HasDatabaseName("ix_medias_media_type");
        builder.HasIndex(m => m.UploadedBy).HasDatabaseName("ix_medias_uploaded_by");
        builder.HasIndex(m => new { m.IsTemp, m.CreatedAtUtc }).HasDatabaseName("ix_medias_temp_cleanup");
    }
}
