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
    public class OrderRepository : GenricRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<Order> UpdateAsync(Order order)
        {
            var orderToUpadate = _db.Orders.FirstOrDefault(b => b.Id ==order.Id);
            if (orderToUpadate == null)
                throw new ArgumentNullException(nameof(order));
            _db.Entry(orderToUpadate).CurrentValues.SetValues(order);
            await SaveAsync();
            return orderToUpadate;
        }
    }
}
