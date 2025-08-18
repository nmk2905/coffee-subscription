using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //GET

        [HttpGet("get-all-category")]
        public async Task<IActionResult> GetAllCategoryAsync()
        {
            var result = await _categoryService.GetAllCategoryAsync();
            return Ok(result);
        }

        [HttpGet("get-category-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Not found!" });
            }
            return Ok(result);
        }

    }
}
