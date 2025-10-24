using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using solidhardware.storeCore.Domain.Entites;

namespace solidhardware.storeinfrastraction.Configration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // 🔹 Table & Primary Key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .ValueGeneratedNever();

            // 🔹 Basic Properties
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Price)
                   .HasPrecision(18, 2) // مهم لتجنب التحذير
                   .IsRequired();

            builder.Property(p => p.Stock_quantity)
                   .IsRequired();

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(300);

            builder.Property(p => p.Brand)
                   .HasMaxLength(100);

            // 🔹 Relationships

            // Product → Category (Many-to-One)
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Product → ProductSpecialProperty (One-to-One)
            builder.HasOne(p => p.ProductSpecialProperty)
                   .WithOne(psp => psp.Product)
                   .HasForeignKey<ProductSpecialProperty>(psp => psp.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Product → BundleItems (One-to-Many)
            builder.HasMany(p => p.BundleItems)
                   .WithOne(bi => bi.Product)
                   .HasForeignKey(bi => bi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Product → OrderItems (One-to-Many)
            builder.HasMany(p => p.OrderItems)
                   .WithOne(oi => oi.Product)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Product → CartItems (One-to-Many)
            builder.HasMany(p => p.CartItems)
                   .WithOne(ci => ci.Product)
                   .HasForeignKey(ci => ci.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Product → Reviews (One-to-Many)
            builder.HasMany(p => p.Reviews)
                   .WithOne(r => r.Products)
                   .HasForeignKey(r => r.prouductId)
                   .OnDelete(DeleteBehavior.Cascade);

           
            builder.HasMany(p => p.WishlistItems)
                   .WithOne(wi => wi.Product)
                   .HasForeignKey(wi => wi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            
            builder.ToTable("Products");
        }
    }
}
