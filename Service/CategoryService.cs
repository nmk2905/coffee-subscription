using Contracts.DTOs.Category;
using Repo.Models;
using Repositories;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryDTO>> GetAllCategoryAsync()
        {
            var result = await _categoryRepository.GetAllCategoryAsync();

            return result?.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList() ?? new List<CategoryDTO>();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var result = await _categoryRepository.GetBaristaById(id);
            return new CategoryDTO
            {
                CategoryId = result.CategoryId,
                Name = result.Name,
                Description = result.Description
            };
        }
    }
}
