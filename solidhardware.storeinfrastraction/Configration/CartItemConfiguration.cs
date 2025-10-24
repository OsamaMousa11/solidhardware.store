using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Configration
{
    public class CartItemConfiguration : IEntityTypeConfiguration<Cartitem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cartitem> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id).ValueGeneratedNever();


            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            builder.Property(ci => ci.UnitPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                   .WithMany()
                   .HasForeignKey(ci => ci.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
