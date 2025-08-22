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
    public class productsController : Controller
    {
        private readonly IProductService _productService;
        public productsController(IProductService productService)
        {
            _productService = productService;
        }

        //GET

        /// <summary>
        /// get-all-products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductAsync();
            return Ok(result);
        }

        /// <summary>
        /// get-product-by-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductById(id);
            if (result == null)
            {
                return NotFound(new { message = "No products found!" });
            }
            return Ok(result);
        }

        /// <summary>
        /// get-coffee-product
        /// </summary>
        /// <returns></returns>
        [HttpGet("coffees")]
        public async Task<IActionResult> GetAllProductCoffeeAsync()
        {
            var result = await _productService.GetAllProductCoffeeAsync();
            return Ok(result);
        }

        /// <summary>
        /// get-freeze-product
        /// </summary>
        /// <returns></returns>
        [HttpGet("freezes")]
        public async Task<IActionResult> GetAllProductFreezeAsync()
        {
            var result = await _productService.GetAllProductFreezeAsync();
            return Ok(result);
        }

        /// <summary>
        /// get-tea-product
        /// </summary>
        /// <returns></returns>
        [HttpGet("teas")]
        public async Task<IActionResult> GetAllProductTeaAsync()
        {
            var result = await _productService.GetAllProductTeaAsync();
            return Ok(result);
        }

        //POST

        /// <summary>
        /// add-product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
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

        /// <summary>
        /// update-product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
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

        /// <summary>
        /// delete-product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
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
