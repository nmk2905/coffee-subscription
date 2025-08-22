using Contracts.DTOs.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Subscription
{
    public class SubReponse
    {
        public string Message { get; set; }
        public SubscriptionDTO Data { get; set; }
    }
}
