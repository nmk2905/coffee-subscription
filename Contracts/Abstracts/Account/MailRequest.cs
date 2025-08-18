using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Abstracts.Account
{
    public class MailRequest
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
    }
}
