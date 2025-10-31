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
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Key).HasMaxLength(200).IsRequired();
      
            builder.Property(p => p.Value).HasMaxLength(200).IsRequired();

            builder.Property(p => p.Size);

            builder.Property(p => p.Unit);



            builder.HasOne(p => p.Product)
        .WithMany(p => p.ProductSpecialProperty).HasForeignKey(P => P.ProductId)

        .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
