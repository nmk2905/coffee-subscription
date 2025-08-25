using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.checkout
{
    public class CheckoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime RedeemedAt { get; set; }
        public string PlanName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
