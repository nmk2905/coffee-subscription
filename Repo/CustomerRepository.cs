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
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository() { }

        public async Task<Customer> GetCustomerEmail(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        private byte[] HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<List<Customer>> GetAllCustomerAsync()
        {
            return _context.Customers.ToList();
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            return await _context.Customers.AnyAsync(e => e.Email == email);
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(i => i.CustomerId == id);
        }

        public async Task<Customer> Verify(string token)
        {
            return await _context.Customers.FirstOrDefaultAsync(t => t.VerificationToken == token);
        }
    }
}
