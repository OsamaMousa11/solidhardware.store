using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.ServiceContract;
using solidhardware.storeCore.IUnitofWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using solidhardware.storeCore.Domain.IRepositoryContract;

namespace solidhardware.storeCore.Service
{
    public class CategoryService : ICategoryService
    {
     
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ILogger<CategoryService> logge, IUnitOfWork unitOfWork, IMapper mapper , ICategoryRepository categoryRepository)
        {
       
            _logger = logge;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryAddRequest? categoryAddRequest)
        {
            _logger.LogInformation("Attempting to create new category: {CategoryName}", categoryAddRequest?.Name);

            if (categoryAddRequest == null || string.IsNullOrWhiteSpace(categoryAddRequest.Name))
            {
                _logger.LogError("CategoryAddRequest object is missing or name is invalid.");
                throw new ArgumentNullException(nameof(categoryAddRequest));
            }

            ValidationHelper.ValidateModel(categoryAddRequest);

            var existingCategory = await _unitOfWork.Repository<Category>()
                .GetByAsync(c => c.Name.ToLower() == categoryAddRequest.Name.ToLower());

            if (existingCategory != null)
            {
                _logger.LogWarning("Category with the same name already exists: {CategoryName}", categoryAddRequest.Name);
                throw new InvalidOperationException("Category with the same name already exists.");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
               {
                try
                {
                    var category = _mapper.Map<Category>(categoryAddRequest);
                    category.Id = Guid.NewGuid();

                    await _unitOfWork.Repository<Category>().CreateAsync(category);
                    await _unitOfWork.CompleteAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation("Category created successfully: {CategoryName}", categoryAddRequest.Name);

                    return _mapper.Map<CategoryResponse>(category);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create category {CategoryName}", categoryAddRequest.Name);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }


        public async Task<bool> DeleteCategory(Guid? id)
        {
            _logger.LogInformation("Deleting Category with ID: {Category}", id);

            if (id == null || id == Guid.Empty)
            {
                _logger.LogError("Invalid category ID provided for deletion");
                throw new ArgumentNullException(nameof(id), "Category ID cannot be null or empty.");
            }

            var category = await _unitOfWork.Repository<Category>().GetByAsync(c=>c.Id == id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID: {Category} not found", id);
                throw new Exception("Category not found");
             
            }

            try
            {
                var result = await _unitOfWork.Repository<Category>().DeleteAsync(category);
                _logger.LogInformation("Category with ID: {Category} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting Category with ID: {Category}", id);
                              return false;
            }
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategories(int pageIndex = 1, int pageSize = 10)
        {
          
            _logger.LogInformation("Fetching all categories. PageIndex: {PageIndex}, PageSize: {PageSize}", pageIndex, pageSize);
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync(pageIndex: pageIndex, pageSize: pageSize);
            return categories.Select(category => _mapper.Map<CategoryResponse>(category));
        }

        public async Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> predicate, bool IsTracked = true)
        {
            _logger.LogInformation("Fetching genre with predicate: {Predicate}", predicate);

    
            var category = await _unitOfWork.Repository<Category>()
                .GetByAsync(predicate, IsTracked, includeProperties: "Category,ProductSpecialProperty");
           if(category == null)
            {
                _logger.LogWarning("Category not found with the given predicate: {Predicate}", predicate);
                throw new Exception("Category not found");
            }


            _logger.LogInformation("Category fetched successfully: {@Category}", category);
            return _mapper.Map<CategoryResponse>(category);

        }

        public  async Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest? categoryUpdateRequest)
        {
            _logger.LogInformation("Updating Category ....", categoryUpdateRequest);
            if (categoryUpdateRequest==null || string.IsNullOrWhiteSpace(categoryUpdateRequest.Name))
            {
                _logger.LogError("Category not Valid");
                throw new ArgumentNullException(nameof(categoryUpdateRequest));
            }
            ValidationHelper.ValidateModel(categoryUpdateRequest);
            var existingcategory = await _unitOfWork.Repository<Category>().GetByAsync(c => c.Name == categoryUpdateRequest.Name && c.Id != categoryUpdateRequest.Id);
            if (existingcategory != null)
            {
              _logger.LogError("Genre name already exists: {CategoryName)", categoryUpdateRequest.Name);
                throw new Exception("Category already exists");
            }
           
            using(var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    
                    var Category = _mapper.Map<Category>(categoryUpdateRequest);
                    var Categoyresult = await _categoryRepository.UpdateAsync(Category);
                    if (Categoyresult == null)
                    {
                        throw new Exception("Failed to update category");
                    }
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Category updated successfully: {CategoryName}", categoryUpdateRequest.Name);
                    return _mapper.Map<CategoryResponse>(Category);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update category {CategoryName}", categoryUpdateRequest.Name);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
