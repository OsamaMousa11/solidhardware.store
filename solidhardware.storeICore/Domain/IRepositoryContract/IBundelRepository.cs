using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.IRepositoryContract
{
    public interface IBundelRepository:IGenericRepository<Bundle>
    {
        Task<Bundle> UpdateAsnc(Bundle bundle);
    }
}
