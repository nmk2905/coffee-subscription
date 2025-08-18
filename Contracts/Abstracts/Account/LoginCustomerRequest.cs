using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Abstracts.Account
{
    public class LoginCustomerRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
