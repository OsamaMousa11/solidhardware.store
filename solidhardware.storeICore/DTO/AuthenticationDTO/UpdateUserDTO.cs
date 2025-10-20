using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.AuthenticationDTO
{
    public class UpdateUserDTO
    {
        public string Id { get; set; } = default!;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NewPassword { get; set; }

    }
}
