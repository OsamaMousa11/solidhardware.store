
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IdentityEntites;
using solidhardware.storeCore.DTO.AuthenticationDTO;
using solidhardware.storeinfrastraction.Configration;

namespace solidhardware.storeinfrastraction.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser, ApplicationRole,Guid>
    {
       public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }        
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Cartitem> Cartitems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<BundleItem> BundleItems { get; set; }
        public DbSet<ProductSpecialProperty> ProductSpecialProperties { get; set; }








        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
      
        }
    }
}
