using Contracts.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.Product
{
    public class ProductResponse
    {
        public string Message { get; set; }
        public ProductDTO Data { get; set; }
    }
}
