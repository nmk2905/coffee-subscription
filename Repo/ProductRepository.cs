using Microsoft.EntityFrameworkCore;
using Repo.Models;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository() { }
        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(i => i.ProductId == id);
        }
        public async Task<List<Product>> GetAllProductCoffeeAsync()
        {
            return await _context.Products
                .Include(p => p.Category).Where(p => p.Category.Name == "Cà Phê")
                .ToListAsync();
        }
        public async Task<List<Product>> GetAllProductFreezeAsync()
        {
            return await _context.Products
                .Include(p => p.Category).Where(p => p.Category.Name == "Freeze")
                .ToListAsync();
        }
        public async Task<List<Product>> GetAllProductTeaAsync()
        {
            return await _context.Products
                .Include(p => p.Category).Where(p => p.Category.Name == "Trà")
                .ToListAsync();
        }
    }
}
