using CodeCafe.Modules.Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for the <see cref="User"/> entity. Lives in infrastructure
/// because persistence detail (column types, indexes, schema) is not a
/// domain concern.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                raw => CodeCafe.Modules.Platform.Domain.ValueObjects.Email.Create(raw))
            .HasColumnName("Email")
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasColumnName("DisplayName")
            .HasMaxLength(120);

        builder.Property(u => u.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .IsRequired();
    }
}
