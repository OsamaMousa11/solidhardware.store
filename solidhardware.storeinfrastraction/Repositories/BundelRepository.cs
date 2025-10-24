using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeinfrastraction.Repositories
{
    public class BundelRepository : GenricRepository<Bundle>, IBundelRepository
    {   
        private readonly AppDbContext _db;
        public BundelRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async  Task<Bundle> UpdateAsnc(Bundle bundle)
        {
           var BundleTOUpdate= _db.Bundles.FirstOrDefault(b => b.Id == bundle.Id);
           if(BundleTOUpdate == null)
                throw new ArgumentNullException(nameof(bundle));

           _db.Entry(BundleTOUpdate).CurrentValues.SetValues(bundle);
              await  SaveAsync();
            return BundleTOUpdate;


        }
    }
}
