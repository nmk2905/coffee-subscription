using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Payment
{
    public class CreatePaymentRequest
    {
        public int SubscriptionId { get; set; }
    }
}
