using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.DTO.ProductDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class ProductService:IProductService
         
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(ILogger<ProductService> logger , IUnitOfWork unitOfWork , IMapper mapper, IProductRepository productRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepository=productRepository;
        }

    

        public async Task<ProductResponse> CreateProductAsync(ProductAddRequest request)
        {

            _logger.LogInformation("Creating a new product");

            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("ProductAddRequest object is missing or name is invalid.");
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ValidateModel(request);

            // check duplicate name
            var existingProduct = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Name.ToLower() == request.Name.ToLower());

            if (existingProduct != null)
            {
                _logger.LogWarning("Product with the same name already exists: {ProductName}", request.Name);
                throw new InvalidOperationException("Product with the same name already exists.");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var product = _mapper.Map<Product>(request);
                    product.Id = Guid.NewGuid();

                    if (request.SpecialProperties != null && request.SpecialProperties.Any())
                    {

                             var specialProperties = request.SpecialProperties
                                   .Select(sp =>
                                   {
                                       var special = _mapper.Map<ProductSpecialProperty>(sp);
                                       special.Id = Guid.NewGuid();
                                       special.ProductId = product.Id;
                                       return special;
                                   })
                                   .ToList();
                        product.ProductSpecialProperty = specialProperties;
                    }

              

                    await _unitOfWork.Repository<Product>().CreateAsync(product);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

                 
                    var createdProduct = await _unitOfWork.Repository<Product>()
                         .GetByAsync(p => p.Id == product.Id, includeProperties: "Category,ProductSpecialProperty");

               
                 return _mapper.Map<ProductResponse>(createdProduct);

                
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating product");
                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }

        public async Task<ProductResponse> GetProduct(Expression<Func<Product, bool>> predicate, bool IsTracked = true)
        {
            _logger.LogInformation($"GeProduct: {predicate}");

            var product = await _unitOfWork.Repository<Product>().GetByAsync(predicate, IsTracked);
            if (product == null)
            {
                _logger.LogWarning("Product not found for the given predicate");
                throw new Exception("Product not found");
            }
            _logger.LogInformation("Product retrieved successfully");
            return _mapper.Map<ProductResponse>(product);

        }

       public async Task<IEnumerable<ProductResponse>> GetAllProducts(int pageIndex = 1, int pageSize = 10)
       {
            _logger.LogInformation("GetAllProducts called");

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(pageIndex: pageIndex, pageSize: pageSize, includeProperties: "Category,ProductSpecialProperty");

            _logger.LogInformation("Products retrieved successfully");

            return _mapper.Map<IEnumerable<ProductResponse>>(products);
       }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
           _logger.LogInformation("DeleteProductAsync called for ProductId: {ProductId}", productId);
            var product = await _unitOfWork.Repository<Product>().GetByAsync(p => p.Id == productId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion with ID: {ProductId}", productId);
                throw new Exception("Product not found");
            }
            var result = await _unitOfWork.Repository<Product>().DeleteAsync(product);
            if (result)
            {
                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", productId);
            }
            else
            {
                _logger.LogWarning("Failed to delete product with ID: {ProductId}", productId);
            }
            return result;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategory(Guid categoryId)
        {
            _logger.LogInformation("GetProductsByCategory called for CategoryId: {CategoryId}", categoryId);
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p => p.CategoryId == categoryId, includeProperties: "Category,ProductSpecialProperty");
            if(products == null )
            {
                _logger.LogWarning("No products found for CategoryId: {CategoryId}", categoryId);
                return Enumerable.Empty<ProductResponse>();
            }
           _logger.LogInformation("Products retrieved successfully for CategoryId: {CategoryId}", categoryId);
            return _mapper.Map<IEnumerable<ProductResponse>>(products);

        }

        public async Task<IEnumerable<ProductResponse>> SearchProducts(string searchTerm)
        {
            _logger.LogInformation("SearchProducts called with searchTerm: {SearchTerm}", searchTerm);
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm),
                includeProperties: "Category,ProductSpecialProperty");
            if (products == null)
            {
              _logger.LogWarning("No products found matching searchTerm: {SearchTerm}", searchTerm);
                return Enumerable.Empty<ProductResponse>();
            }
            else
                {
                _logger.LogInformation("Products retrieved successfully matching searchTerm: {SearchTerm}", searchTerm);
                return _mapper.Map<IEnumerable<ProductResponse>>(products);
            }

        }

        public  async Task<bool> UpdateStock(Guid id, int quantity)
        {
           
            _logger.LogInformation("UpdateStock called for ProductId: {ProductId} with Quantity: {Quantity}", id, quantity);
            if(quantity < 0 )
            {
                _logger.LogError("Invalid stock quantity: {Quantity} for ProductId: {ProductId}", quantity, id);
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
            }
         
            var product = await _unitOfWork.Repository<Product>().GetByAsync(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for stock update with ID: {ProductId}", id);
                throw new Exception("Product not found");
            }
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {   product.Stock_quantity = quantity;
                    await _productRepository.UpdateAsync(product);
                    await  _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Stock updated successfully for ProductId: {ProductId} to Quantity: {Quantity}", id, quantity);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating stock for ProductId: {ProductId}", id);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

       
            public async Task<ProductResponse> UpdateProduct(ProductUpdateRequest updateRequest)
        {
            _logger.LogInformation("UpdateProduct called");

            if (updateRequest == null)
                throw new ArgumentNullException(nameof(updateRequest));

            if (string.IsNullOrWhiteSpace(updateRequest.Name))
                throw new ArgumentException("Product name is invalid");

            ValidationHelper.ValidateModel(updateRequest);

            var product = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Id == updateRequest.Id, includeProperties: "Category,ProductSpecialProperty");

            if (product == null)
            {
                _logger.LogWarning("Product not found with Id: {Id}", updateRequest.Id);
                throw new InvalidOperationException("Product not found");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _mapper.Map(updateRequest, product);

                  
                    if (updateRequest.SpecialProperties != null)
                    {
                        foreach (var propDto in updateRequest.SpecialProperties)
                        {
                            var existingProp = product.ProductSpecialProperty
                                .FirstOrDefault(p => p.Id == propDto.Id);

                            if (existingProp == null)
                            {
                            
                                var newProp = _mapper.Map<ProductSpecialProperty>(propDto);
                                newProp.Id = Guid.NewGuid();
                                newProp.ProductId = product.Id;
                                await _unitOfWork.Repository<ProductSpecialProperty>().CreateAsync(newProp);
                            }
                            else
                            {
                              
                                _mapper.Map(propDto, existingProp);
                            }
                        }
                    }

                    await _productRepository.UpdateAsync(product);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Product updated successfully with ID: {ProductId}", product.Id);
                    return _mapper.Map<ProductResponse>(product);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

    }



}
    

