using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Repositories
{
    public class ReviewRepository : GenricRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _db;
        public ReviewRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            var reviewToUpdate = _db.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (reviewToUpdate == null)
                throw new ArgumentNullException(nameof(review));

            _db.Entry(reviewToUpdate).CurrentValues.SetValues(review);
            await SaveAsync();
            return reviewToUpdate;
        }
    }
}
