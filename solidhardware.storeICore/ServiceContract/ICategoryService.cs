using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CategotyDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategory(CategoryAddRequest? categoryAddRequest);
        Task <bool> DeleteCategory(Guid? id);
        Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest? categoryUpdateRequest);
        Task<CategoryResponse>GetCategory(Expression<Func<Category, bool>> predicate, bool IsTracked = true);
        Task <IEnumerable<CategoryResponse>> GetAllCategories(int pageIndex = 1, int pageSize = 10);

    }
}
