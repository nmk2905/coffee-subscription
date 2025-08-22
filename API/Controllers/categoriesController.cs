using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class categoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        public categoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //GET

        /// <summary>
        /// get-all-category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategoryAsync()
        {
            var result = await _categoryService.GetAllCategoryAsync();
            return Ok(result);
        }


        /// <summary>
        /// get-category-by-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
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
