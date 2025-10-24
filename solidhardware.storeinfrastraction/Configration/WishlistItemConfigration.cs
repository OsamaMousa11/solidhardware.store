using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Configration
{
    public class WishlistItemConfigration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasKey(wi => wi.Id);
            builder.Property(wi => wi.Id)
                   .IsRequired();
            builder.HasOne(wi => wi.Wishlist)
                     .WithMany(w => w.WishlistItems)
                     .HasForeignKey(wi => wi.WishlistId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wi => wi.Product)
                   .WithMany(p => p.WishlistItems)
                   .HasForeignKey(wi => wi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
