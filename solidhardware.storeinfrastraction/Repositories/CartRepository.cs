using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Repositories
{
    public class CartRepository : GenricRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _db;
        public CartRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async  Task<Cart> UpdateAsync(Cart cart)
        {
            var CartToUpadate =_db.Carts.FirstOrDefault(b => b.Id == cart.Id);
            if(CartToUpadate == null) 
                throw new ArgumentNullException(nameof(cart));
            _db.Entry(CartToUpadate).CurrentValues.SetValues(cart);
            await SaveAsync();
            return CartToUpadate;


        }
    }
}
