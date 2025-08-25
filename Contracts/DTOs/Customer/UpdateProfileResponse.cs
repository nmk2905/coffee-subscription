using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Customer
{
    public class UpdateProfileResponse
    {
        public string Message { get; set; }
        public UpdateProfileDTO Data { get; set; }
    }
}
