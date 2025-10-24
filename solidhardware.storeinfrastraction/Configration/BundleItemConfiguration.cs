using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Configration
{
    public class BundleItemConfiguration : IEntityTypeConfiguration<BundleItem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BundleItem> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();
            builder.Property(b => b.Quantity).IsRequired();
            builder.Property(b => b.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(bi => bi.Bundle)
                   .WithMany(b => b.BundleItems)
                   .HasForeignKey(bi => bi.BundleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bi => bi.Product)
                   .WithMany(b => b.BundleItems)
                   .HasForeignKey(bi => bi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
