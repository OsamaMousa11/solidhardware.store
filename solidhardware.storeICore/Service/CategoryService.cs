using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.ServiceContract;
using solidhardware.storeCore.IUnitofWork;
using AutoMapper;
using solidhardware.storeCore.Domain.IRepositoryContract;
using System.Linq.Expressions;

namespace solidhardware.storeCore.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(
            ILogger<CategoryService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

 
        // CREATE

        public async Task<CategoryResponse> CreateCategory(CategoryAddRequest categoryAddRequest)
        {
            if (categoryAddRequest == null)
                throw new ArgumentNullException(nameof(categoryAddRequest));

            ValidationHelper.ValidateModel(categoryAddRequest);

            _logger.LogInformation("Creating category {Name}", categoryAddRequest.Name);

            var exists = await _unitOfWork.Repository<Category>()
                .GetByAsync(c => c.Name.ToLower() == categoryAddRequest.Name.ToLower());

            if (exists != null)
                throw new InvalidOperationException("Category with the same name already exists.");

            using var tran = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = _mapper.Map<Category>(categoryAddRequest);
                category.Id = Guid.NewGuid();

                await _unitOfWork.Repository<Category>().CreateAsync(category);
                await _unitOfWork.CompleteAsync();

                await tran.CommitAsync();

                return _mapper.Map<CategoryResponse>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                await tran.RollbackAsync();
                throw;
            }
        }

      
        // DELETE
     
        public async Task<bool> DeleteCategory(Guid? id)
        {
            _logger.LogInformation("Deleting category {Id}", id);

            var category = await _unitOfWork.Repository<Category>().GetByAsync(c => c.Id == id);
            if (category == null)
                throw new Exception("Category not found");

            var result = await _unitOfWork.Repository<Category>().DeleteAsync(category);

            return result; 
        }

     


        // GET ALL

        public async Task<IEnumerable<CategoryResponse>> GetAllCategories(int pageIndex = 1, int pageSize = 10)
        {
            var categories = await _unitOfWork.Repository<Category>()
                .GetAllAsync(pageIndex: pageIndex, pageSize: pageSize);

            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }
        
        // GET BY

        public async Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> predicate, bool isTracked = true)
        {
            var category = await _unitOfWork.Repository<Category>()
                .GetByAsync(predicate, isTracked);

            if (category == null)
                throw new Exception("Category not found");

            return _mapper.Map<CategoryResponse>(category);
        }

  
        // UPDATE
 
        public async Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);

            _logger.LogInformation("Updating category {Id}", request.Id);

            var existingCategory = await _unitOfWork.Repository<Category>()
                .GetByAsync(c => c.Id == request.Id);

            if (existingCategory == null)
                throw new Exception("Category not found");

            var duplicate = await _unitOfWork.Repository<Category>()
                .GetByAsync(c => c.Name == request.Name && c.Id != request.Id);

            if (duplicate != null)
                throw new Exception("Category name already exists.");

            using var tran = await _unitOfWork.BeginTransactionAsync();
            try
            {
               
                _mapper.Map(request, existingCategory);

                var updated = await _categoryRepository.UpdateAsync(existingCategory);

                await _unitOfWork.CompleteAsync();
                await tran.CommitAsync();

                return _mapper.Map<CategoryResponse>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category {Id}", request.Id);
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}
