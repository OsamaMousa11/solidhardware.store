using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Repositories
{
    public class WishListRepository:GenricRepository<Wishlist>,IWishListRepository
    {
        private readonly AppDbContext _db;
        public WishListRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Wishlist> UpdateWishlistAsync(Wishlist updatedWishlist)
        {
            if (updatedWishlist == null)
                throw new ArgumentNullException(nameof(updatedWishlist));

            var existingWishlist = await _db.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.Id == updatedWishlist.Id);

            if (existingWishlist == null)
                throw new KeyNotFoundException("Wishlist not found");

            
          

            if (updatedWishlist.UserId != Guid.Empty)
                existingWishlist.UserId = updatedWishlist.UserId;

        
            if (updatedWishlist.WishlistItems != null)
            {
    
                var itemsToRemove = existingWishlist.WishlistItems
                    .Where(i => !updatedWishlist.WishlistItems.Any(u => u.Id == i.Id))
                    .ToList();
                _db.WishlistItems.RemoveRange(itemsToRemove);

                // Add new items
                var newItems = updatedWishlist.WishlistItems
                    .Where(u => !existingWishlist.WishlistItems.Any(i => i.Id == u.Id))
                    .Select(i => new WishlistItem
                    {
                        Id = Guid.NewGuid(),
                        WishlistId = existingWishlist.Id,
                        ProductId = i.ProductId,
                      
                    })
                    .ToList();
                await _db.WishlistItems.AddRangeAsync(newItems);

           
            }

            _db.Wishlists.Update(existingWishlist);
            await _db.SaveChangesAsync();

            return existingWishlist;
        }
    }

}

