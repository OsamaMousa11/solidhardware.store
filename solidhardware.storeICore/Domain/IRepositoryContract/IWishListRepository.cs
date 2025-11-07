using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.IRepositoryContract
{
    public interface IWishListRepository:IGenericRepository<Wishlist>
    {
        Task<Wishlist> UpdateWishlistAsync(Wishlist updatedWishlist);
    }
}
