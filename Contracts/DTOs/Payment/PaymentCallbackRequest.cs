using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Payment
{
    public class PaymentCallbackRequest
    {
        public string TransactionCode { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; } // SUCCESS, FAILED
        public decimal Amount { get; set; }
    }
}
