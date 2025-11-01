using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.BundleDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace solidhardware.storeCore.Service
{
    public class BundleService: IBundleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BundleService> _logger;
        private readonly IMapper _mapper;
        private readonly IBundelRepository _BundleRepository;

        public BundleService(IUnitOfWork unitOfWork , ILogger<BundleService> logger , IBundelRepository BundleRepository , IMapper mapper)
        {
            _BundleRepository =BundleRepository;
            _logger=logger;
            _unitOfWork=unitOfWork;
            _mapper=mapper;

        }

        public  async Task<BundleResponse> CreateAsync(BundleAddRequest Bundeladdrequest)
        {
            _logger.LogInformation("Creating a new bundle");
            if(Bundeladdrequest == null)
            {
                _logger.LogError("BundleAddRequest is null");
                throw new ArgumentNullException(nameof(Bundeladdrequest));
            }
            if(string.IsNullOrEmpty(Bundeladdrequest.Name))
                {
                _logger.LogError("Bundle name is null or empty");
                throw new ArgumentException("Bundle name is required", nameof(Bundeladdrequest.Name));
            }
            ValidationHelper.ValidateModel(Bundeladdrequest);
            var existingBundle = await _BundleRepository.GetByAsync(b => b.Name == Bundeladdrequest.Name);
            if(existingBundle != null)
            {
                _logger.LogError("Bundle with name {BundleName} already exists", Bundeladdrequest.Name);
                throw new InvalidOperationException($"Bundle with name {Bundeladdrequest.Name} already exists");
            }
            using (var transaction = await _unitOfWork.BeginTransactionAsync())

            {
                try
                {
                    var bundleEntity = _mapper.Map<Bundle>(Bundeladdrequest);
                    bundleEntity.Id = Guid.NewGuid();

                    if(bundleEntity.BundleItems != null && bundleEntity.BundleItems.Count > 0)
                    {
                     var BundleItems = Bundeladdrequest.BundleItems.Select(Bi=>
                     {
                         var bundleItemEntity = _mapper.Map<BundleItem>(Bi);
                            bundleItemEntity.Id = Guid.NewGuid();
                            bundleItemEntity.BundleId = bundleEntity.Id;
                            return bundleItemEntity;
                     }).ToList();
                        bundleEntity.BundleItems = BundleItems;

                    }

                    await _unitOfWork.Repository<Bundle>().CreateAsync(bundleEntity);

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Bundle created successfully with id {BundleId}", bundleEntity.Id);
                    var createdBundle = await _unitOfWork.Repository<Bundle>().GetByAsync(b => b.Id == bundleEntity.Id, includeProperties: "BundleItems.Product");
                    return _mapper.Map<BundleResponse>(createdBundle);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating bundle");
                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }

        public async  Task<bool> DeleteAsync(Guid id)
        {
             _logger.LogInformation("Deleting bundle with id {BundleId}", id);
            var bundleEntity = await _unitOfWork.Repository<Bundle>().GetByAsync(x=>x.Id==id);
            if (bundleEntity == null)
            {
                _logger.LogError("Bundle with id {BundleId} not found", id);
                return false;
            }
           var result= await _unitOfWork.Repository<Bundle>().DeleteAsync(bundleEntity);
            if(!result)
            {
               _logger.LogError("Failed to delete bundle with id {BundleId}", id);
                return false;
            }
          
            _logger.LogInformation("Bundle with id {BundleId} deleted successfully", id);
            return true;

        }

        public  async Task<IEnumerable<BundleResponse>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
           _logger.LogInformation( "Fetching all bundles");
            var bundles = await _unitOfWork.Repository<Bundle>().GetAllAsync(pageIndex: pageIndex, pageSize: pageSize,includeProperties: "BundleItems.Product.Category,BundleItems.Product.SpecialProperties");
            if (bundles == null || !bundles.Any())
            {
                _logger.LogInformation("No bundles found");
                return Enumerable.Empty<BundleResponse>();
            }

            _logger.LogInformation("Fetched {BundleCount} bundles", bundles.Count());
            _logger.LogInformation("bundles retrieved successfully");

           return _mapper.Map<IEnumerable<BundleResponse>>(bundles);
          
        }

        public  async Task<BundleResponse?> GetAsync(Expression<Func<Bundle, bool>> predicate, bool IsTracked = true)
        {  
       
         _logger.LogInformation("Fetching bundle with specified predicate");

            var bundles = await _unitOfWork.Repository<Bundle>().GetByAsync(
           predicate, IsTracked, includeProperties: "BundleItems.Product.Category,BundleItems.Product.SpecialProperties");
            if (bundles == null)
            {
                _logger.LogWarning("No bundle found matching the specified predicate");
                return null;
            }
            _logger.LogInformation("Bundle found matching the specified predicate");
            return _mapper.Map<BundleResponse>(bundles);
        }

        public async Task<BundleResponse> UpdateAsync(BundleUpdateRequest bundleupdaterequest)
        {
            _logger.LogInformation("UpdateBundle called");

            if (bundleupdaterequest == null)
                throw new ArgumentNullException(nameof(bundleupdaterequest));

            if (string.IsNullOrWhiteSpace(bundleupdaterequest.Name))
                throw new ArgumentException("Bundle name is invalid", nameof(bundleupdaterequest.Name));

            ValidationHelper.ValidateModel(bundleupdaterequest);

            var bundle = await _unitOfWork.Repository<Bundle>()
                .GetByAsync(b => b.Id == bundleupdaterequest.Id, includeProperties: "BundleItems.Product");

            if (bundle == null)
            {
                _logger.LogWarning("Bundle not found with Id: {Id}", bundleupdaterequest.Id);
                throw new InvalidOperationException("Bundle not found");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
   
                    _mapper.Map(bundleupdaterequest, bundle);

   
                    if (bundleupdaterequest.BundleItems != null)
                    {
                        foreach (var itemDto in bundleupdaterequest.BundleItems)
                        {
                            var existingItem = bundle.BundleItems?
                                .FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                            if (existingItem == null)
                            {
                              
                                var newItem = _mapper.Map<BundleItem>(itemDto);
                                newItem.Id = Guid.NewGuid();
                                newItem.BundleId = bundle.Id;
                                await _unitOfWork.Repository<BundleItem>().CreateAsync(newItem);
                            }
                            else
                            {
                          
                                _mapper.Map(itemDto, existingItem);
                            }
                        }
                    }

                    await _BundleRepository.UpdateAsnc(bundle);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Bundle updated successfully with ID: {BundleId}", bundle.Id);

                    return _mapper.Map<BundleResponse>(bundle);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating bundle with ID: {BundleId}", bundleupdaterequest.Id);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

    }
}
