using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.BundleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IBundleService
    {
        Task<bool> DeleteAsync(Guid id);
        Task<BundleResponse?>GetAsync(Expression<Func<Bundle, bool>> predicate, bool IsTracked = true);
        Task<IEnumerable<BundleResponse>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
        Task<BundleResponse> CreateAsync(BundleAddRequest Bundeladdrequest);
        Task<BundleResponse> UpdateAsync( BundleUpdateRequest bundleupdaterequest);

    }
}
