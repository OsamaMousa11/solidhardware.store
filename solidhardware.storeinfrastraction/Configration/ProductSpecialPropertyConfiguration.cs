using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using solidhardware.storeCore.Domain.Entites;

namespace solidhardware.storeinfrastraction.Configration
{
    public class ProductSpecialPropertyConfiguration : IEntityTypeConfiguration<ProductSpecialProperty>
    {
        public void Configure(EntityTypeBuilder<ProductSpecialProperty> builder)
        {
        
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();


            builder.HasOne(p => p.Product)
        .WithOne(p => p.ProductSpecialProperty)
        .HasForeignKey<ProductSpecialProperty>(p => p.ProductId)
        .OnDelete(DeleteBehavior.Cascade);



            builder.Property(p => p.SSD_Type).HasMaxLength(50);
            builder.Property(p => p.Case_Power_Supply).HasMaxLength(100);
            builder.Property(p => p.Color).HasMaxLength(30);
            builder.Property(p => p.PSU_Wattage).HasMaxLength(20);

            builder.Property(p => p.VRAMUnit).HasDefaultValue("GB");
            builder.Property(p => p.SpeedUnit).HasDefaultValue("MHz");
            builder.Property(p => p.RefreshRate_Unit).HasDefaultValue("Hz");
            builder.Property(p => p.ResponseTimeUnit).HasDefaultValue("ms");
            builder.Property(p => p.ScreenSizeUnit).HasDefaultValue("Inch");
        }
    }
}
