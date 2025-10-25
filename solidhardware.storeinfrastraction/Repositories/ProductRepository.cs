using Microsoft.EntityFrameworkCore;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace solidhardware.storeinfrastraction.Repositories
{
    public class ProductRepository : GenricRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }


        public async Task<Product> UpdateAsync(Product product)
        {
            var ProductToUpadate = _db.Products.FirstOrDefault(b => b.Id == product.Id);
            if (ProductToUpadate == null)
                throw new ArgumentNullException(nameof(product));
            _db.Entry(ProductToUpadate).CurrentValues.SetValues(product);
            await SaveAsync();
            return ProductToUpadate;
            


        }
    }
}
