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
    public class CategoryRepository : GenricRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<Category> UpdateAsync(Category category)
        {
            var CategoryToUpadate = _db.Categories.FirstOrDefault(b => b.Id == category.Id);
            if (CategoryToUpadate == null)
                throw new ArgumentNullException(nameof(category));
            _db.Entry(CategoryToUpadate).CurrentValues.SetValues(category);
            await SaveAsync();
            return CategoryToUpadate;


        }
    }
}
