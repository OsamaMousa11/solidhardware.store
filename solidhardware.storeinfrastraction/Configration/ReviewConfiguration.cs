using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IdentityEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Configration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r=>r.Id).ValueGeneratedOnAdd();
            builder.Property(r => r.Comment).IsRequired().HasMaxLength(1000);
            builder.Property(r => r.Rating).IsRequired();

            builder.HasOne(u=>u.Users)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
