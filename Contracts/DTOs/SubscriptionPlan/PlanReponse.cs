using Contracts.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.SubscriptionPlan
{
    public class PlanReponse
    {
        public string Message { get; set; }
        public PlanDTO Data { get; set; }
    }
}
