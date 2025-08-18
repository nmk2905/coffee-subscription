using Contracts.DTOs.Category;
using Contracts.DTOs.Staff;
using Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoryAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
    }
}
