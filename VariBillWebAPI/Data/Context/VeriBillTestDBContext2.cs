





//using Microsoft.EntityFrameworkCore;
//using VariBillWebAPI.Data.Entities;

//namespace VariBillWebAPI.Data.Context;

//public class VeriBillTestDBContext : DbContext
//{
//    public VeriBillTestDBContext(DbContextOptions<VeriBillTestDBContext> options) : base(options)
//    {
//    }

//    public DbSet<Product> Products => Set<Product>();

//    public DbSet<ProductType> ProductTypes => Set<ProductType>();

//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder);

//        builder.Entity<ProductType>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
//            entity.Property(e => e.Description).HasMaxLength(500);
//            entity.HasIndex(e => e.Name).IsUnique();
//        });

//        builder.Entity<Product>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
//            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
//            entity.Property(e => e.Description).HasMaxLength(1000);
//            entity.HasIndex(e => e.Name).IsUnique();
//            entity.HasIndex(e => e.ProductTypeId);

//            entity.HasOne(e => e.ProductType)
//                .WithMany(pt => pt.Products)
//                .HasForeignKey(e => e.ProductTypeId)
//                .OnDelete(DeleteBehavior.Restrict);
//        });
//    }
//}
