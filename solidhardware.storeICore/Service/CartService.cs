using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class CartService : ICartService
    {
        

        public Task<CartResponse> AddItemAsync(CartAddRequest cartAddRequest)
        {
            throw new NotImplementedException();
        }
    }
}
