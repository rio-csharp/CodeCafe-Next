using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for Platform workspaces. The first workspace model is
/// intentionally small: a default personal workspace owned by one user.
/// </summary>
public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasConversion(
                id => id.Value,
                raw => WorkspaceId.From(raw))
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(w => w.OwnerUserId)
            .HasColumnName("OwnerUserId")
            .IsRequired();

        builder.Property(w => w.Name)
            .HasColumnName("Name")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(w => w.Kind)
            .HasConversion<string>()
            .HasColumnName("Kind")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(w => w.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .IsRequired();

        builder.HasIndex(w => new { w.OwnerUserId, w.Kind }).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(w => w.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
