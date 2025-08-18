using Contracts.DTOs.Customer;
using Contracts.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class ProductResponse
    {
        public string Message { get; set; }
        public ProductDTO Data { get; set; }
    }
}
