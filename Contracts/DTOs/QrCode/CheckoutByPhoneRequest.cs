using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.QrCode
{
    public class CheckoutByPhoneRequest
    {
        public string Phone { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
        public int Quantity { get; set; }
    }
}
