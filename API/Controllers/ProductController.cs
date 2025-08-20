using Contracts.DTOs;
using Contracts.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //GET

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductAsync();
            return Ok(result);
        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductById(id);
            if (result == null)
            {
                return NotFound(new { message = "No products found!" });
            }
            return Ok(result);
        }

        [HttpGet("get-coffee-product")]
        public async Task<IActionResult> GetAllProductCoffeeAsync()
        {
            var result = await _productService.GetAllProductCoffeeAsync();
            return Ok(result);
        }

        [HttpGet("get-freeze-product")]
        public async Task<IActionResult> GetAllProductFreezeAsync()
        {
            var result = await _productService.GetAllProductFreezeAsync();
            return Ok(result);
        }

        [HttpGet("get-tea-product")]
        public async Task<IActionResult> GetAllProductTeaAsync()
        {
            var result = await _productService.GetAllProductTeaAsync();
            return Ok(result);
        }

        //POST

        [HttpPost("add-product")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductRequest request)
        {
            var dto = new AddProductDTO
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                Image = request.Image
            };

            var result = await _productService.AddProduct(dto);

            return Ok(result);
        }

        //PUT

        [HttpPut("update-product")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductRequest request)
        {
            var dto = new UpdateProductDTO
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                Image = request.Image
            };

            var result = await _productService.UpdateProduct(dto);

            return Ok(result);
        }

        //DELETE

        [HttpDelete("delete-product/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result)
            {
                return NotFound(new { message = "Product not found!" });
            }
            return Ok(new { message = "Product deleted successfully!" });
        }
    }
}
