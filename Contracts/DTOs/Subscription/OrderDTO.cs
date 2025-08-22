using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Subscription
{
    public class OrderDTO
    {
        public int CustomerId { get; set; }
        public int PlanId { get; set; }
    }
}
