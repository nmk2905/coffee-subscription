using Contracts.DTOs;
using Contracts.DTOs.Product;
using Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductAsync();
        Task<ProductDTO> GetProductById(int id);
        Task<List<ProductDTO>> GetAllProductCoffeeAsync();
        Task<List<ProductDTO>> GetAllProductFreezeAsync();
        Task<List<ProductDTO>> GetAllProductTeaAsync();
        Task<ProductResponse> AddProduct(AddProductDTO request);
        Task<ProductResponse> UpdateProduct(UpdateProductDTO request);
        Task<bool> DeleteProduct(int id);
    }
}
