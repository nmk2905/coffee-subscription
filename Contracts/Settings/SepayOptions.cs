using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Settings
{
    public class SepayOptions
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string CallbackUrl { get; set; }
    }

}
