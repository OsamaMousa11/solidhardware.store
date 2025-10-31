using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync(ProductAddRequest request);
        Task<ProductResponse> GetProduct(Expression<Func<Product, bool>> predicate, bool IsTracked = true);
        Task<IEnumerable<ProductResponse>> GetAllProducts(int pageIndex = 1, int pageSize = 10);
        Task<ProductResponse> UpdateProduct(ProductUpdateRequest request);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<IEnumerable<ProductResponse>> GetProductsByCategory(Guid categoryId);
        Task<IEnumerable<ProductResponse>> SearchProducts(string searchTerm);
        Task<bool> UpdateStock(Guid id, int quantity);
    }
}
