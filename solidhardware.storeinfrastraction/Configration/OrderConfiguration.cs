using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IdentityEntites;

namespace solidhardware.storeinfrastraction.Configration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
   
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                   .ValueGeneratedNever();

            builder.Property(o => o.OrderDate)
                   .IsRequired();

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.City)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.Country)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.Phone)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasOne(o => o.user)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Orders");
        }
    }
}
