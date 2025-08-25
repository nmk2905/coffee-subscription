using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.checkout
{
    // Request checkout từ 1 subscription đã chọn
    public class CheckoutBySubscriptionRequest
    {
        public int SubscriptionId { get; set; }
        public int Quantity { get; set; }
    }
}
