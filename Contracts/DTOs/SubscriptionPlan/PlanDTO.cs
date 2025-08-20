using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.SubscriptionPlan
{
    public class PlanDTO
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int? DailyQuota { get; set; }
        public int? MaxPerVisit { get; set; }
        public bool? Active { get; set; }
    }
}
