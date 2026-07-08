using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Data.Entities;

namespace VariBillWebAPI.Data.Context;

/// <summary>
/// Entity Framework Core database context.
/// </summary>
public class VeriBillTestDBContext : DbContext
{
    public VeriBillTestDBContext(DbContextOptions<VeriBillTestDBContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductType> ProductTypes => Set<ProductType>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ProductType configuration
        builder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Product configuration
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.ProductTypeId);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(e => e.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityName).HasMaxLength(200);
            entity.Property(e => e.EntityId).HasMaxLength(50);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.HasIndex(e => e.EntityName);
            entity.HasIndex(e => e.Timestamp);
            // Optionally, add index on (EntityName, EntityId) for faster lookups
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
        });
    }
}



