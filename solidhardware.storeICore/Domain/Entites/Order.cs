using solidhardware.storeCore.Domain.IdentityEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser user { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
