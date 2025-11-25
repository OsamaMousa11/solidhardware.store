using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.ProductDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        private const string PRODUCT_INCLUDES = "Category,ProductSpecialProperty";

        public ProductService(
            ILogger<ProductService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IProductRepository productRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        public async Task<ProductResponse> CreateProductAsync(ProductAddRequest request)
        {
            _logger.LogInformation("Creating product");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);

            var exists = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Name.ToLower() == request.Name.ToLower());

            if (exists != null)
                throw new InvalidOperationException("Product with this name already exists.");

            using var trx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var product = _mapper.Map<Product>(request);
                product.Id = Guid.NewGuid();

                // special props
                if (request.SpecialProperties != null)
                {
                    product.ProductSpecialProperty = request.SpecialProperties.Select(sp =>
                    {
                        var entity = _mapper.Map<ProductSpecialProperty>(sp);
                        entity.Id = Guid.NewGuid();
                        entity.ProductId = product.Id;
                        return entity;
                    }).ToList();
                }

                await _unitOfWork.Repository<Product>().CreateAsync(product);
                await _unitOfWork.CompleteAsync();
                await trx.CommitAsync();

                var created = await _unitOfWork.Repository<Product>()
                    .GetByAsync(p => p.Id == product.Id, includeProperties: PRODUCT_INCLUDES);

                return _mapper.Map<ProductResponse>(created);
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        // ------------------------------------------------------------
        // GET BY PREDICATE
        // ------------------------------------------------------------
        public async Task<ProductResponse> GetProduct(Expression<Func<Product, bool>> predicate, bool IsTracked = true)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetByAsync(predicate, IsTracked, includeProperties: PRODUCT_INCLUDES);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return _mapper.Map<ProductResponse>(product);
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        public async Task<IEnumerable<ProductResponse>> GetAllProducts(int pageIndex = 1, int pageSize = 10)
        {
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(pageIndex: pageIndex, pageSize: pageSize, includeProperties: PRODUCT_INCLUDES);

            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            var result = await _unitOfWork.Repository<Product>().DeleteAsync(product);

            return result;
        }

        // ------------------------------------------------------------
        // GET BY CATEGORY
        // ------------------------------------------------------------
        public async Task<IEnumerable<ProductResponse>> GetProductsByCategory(Guid categoryId)
        {
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p => p.CategoryId == categoryId, includeProperties: PRODUCT_INCLUDES);

            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        // ------------------------------------------------------------
        // SEARCH
        // ------------------------------------------------------------
        public async Task<IEnumerable<ProductResponse>> SearchProducts(string searchTerm)
        {
            searchTerm = searchTerm?.Trim() ?? "";

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(
                    p => p.Name.Contains(searchTerm) ||
                         p.Description.Contains(searchTerm),
                    includeProperties: PRODUCT_INCLUDES
                );

            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        // ------------------------------------------------------------
        // UPDATE STOCK
        // ------------------------------------------------------------
        public async Task<bool> UpdateStock(Guid id, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            var product = await _unitOfWork.Repository<Product>().GetByAsync(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            using var trx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                product.Stock_quantity = quantity;

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.CompleteAsync();
                await trx.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                throw;
            }
        }

        // ------------------------------------------------------------
        // UPDATE PRODUCT
        // ------------------------------------------------------------
        public async Task<ProductResponse> UpdateProduct(ProductUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);

            var product = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Id == request.Id, includeProperties: PRODUCT_INCLUDES);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            using var trx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                _mapper.Map(request, product);

                // handle special props
                if (request.SpecialProperties != null)
                {
                    foreach (var propDto in request.SpecialProperties)
                    {
                        var existing = product.ProductSpecialProperty
                            .FirstOrDefault(p => p.Id == propDto.Id);

                        if (existing != null)
                        {
                            _mapper.Map(propDto, existing);
                        }
                        else
                        {
                            var newProp = _mapper.Map<ProductSpecialProperty>(propDto);
                            newProp.Id = Guid.NewGuid();
                            newProp.ProductId = product.Id;
                            await _unitOfWork.Repository<ProductSpecialProperty>().CreateAsync(newProp);
                        }
                    }
                }

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.CompleteAsync();
                await trx.CommitAsync();

                return _mapper.Map<ProductResponse>(product);
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
