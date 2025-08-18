using Contracts.DTOs;
using Contracts.DTOs.Product;
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
    }
}
