using solidhardware.storeCore.Domain.IdentityEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
     
        public int Rating { get; set; }

        public Guid prouductId { get; set; }
        public Product Products { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser Users { get; set; }
        public DateTime ReviewDate { get; set; }

    }
}
