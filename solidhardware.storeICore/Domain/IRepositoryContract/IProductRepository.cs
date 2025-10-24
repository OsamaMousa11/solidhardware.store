using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.IRepositoryContract
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<Product> UpdateAsync( Product product);
    }
}
