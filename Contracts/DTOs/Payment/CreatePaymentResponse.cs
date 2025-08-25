using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Payment
{
    public class CreatePaymentResponse
    {
        public int PaymentId { get; set; }
        public long Amount { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string TransferContent { get; set; }
        public string QrImageUrl { get; set; }
    }
}
