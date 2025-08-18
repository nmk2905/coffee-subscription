using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Abstracts.Account
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
