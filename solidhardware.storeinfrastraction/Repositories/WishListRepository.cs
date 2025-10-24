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
    }
}
