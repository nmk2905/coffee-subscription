using Contracts.DTOs.Product;
using Contracts.DTOs.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class PlanReponse
    {
        public string Message { get; set; }
        public PlanDTO Data { get; set; }
    }
}
