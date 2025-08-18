using Microsoft.EntityFrameworkCore;
using Repo.Models;
using Repositories.Base;


namespace Repositories
{
    public class StaffRepository : GenericRepository<Staff>
    {
        public StaffRepository() { }

        public async Task<Staff> GetStaffAccount(string email, string password)
        {
            return await _context.Staff.Where(s => s.Email == email && s.Password == password)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Staff>> GetAllBaristaAsync()
        {
            return _context.Staff.Where(s => s.Role == "Barista").Include(e => e.Store).ToList();
        }
        public async Task<bool> CheckEmailExist(string email)
        {
            return await _context.Staff.AnyAsync(e => e.Email == email);
        }
        public async Task<Staff> GetBaristaById(int id)
        {
            return await _context.Staff.Where(s => s.Role == "Barista")
                .Include(e => e.Store).FirstOrDefaultAsync(i => i.StaffId == id);
        }
    }
}
