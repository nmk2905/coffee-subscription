using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Subscription
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? PlanId { get; set; }
        public string PlanName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? RemainingDays { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
    }
}
