using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Configration
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
           builder.HasKey(w => w.Id);
            builder.Property(w => w.Id)
                   .IsRequired();

            builder.HasOne(w => w.User)
       .WithMany(u => u.Wishlists)
       .HasForeignKey(w => w.UserId)
       .OnDelete(DeleteBehavior.Cascade);

     



        }
    }
}
