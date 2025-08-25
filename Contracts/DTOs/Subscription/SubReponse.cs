using Contracts.DTOs.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Subscription
{
    public class SubResponse
    {
        public string Message { get; set; }
        public SubscriptionDTO Data { get; set; }
        public string QrUrl { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string AccountHolder { get; set; }
        public string TransferContent { get; set; }
        public decimal Amount { get; set; }
    }
}
