using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.IRepositoryContract
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        Task<Category> UpdateAsync(Category category);

    }
}
