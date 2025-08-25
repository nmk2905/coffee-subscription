using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.checkout
{
    // Response: danh sách subscription để staff chọn
    public class SubscriptionInfoResponse
    {
        public int SubscriptionId { get; set; }
        public string PlanName { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? RemainingDays { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
    }
}
