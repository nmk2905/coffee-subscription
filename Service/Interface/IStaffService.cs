using Contracts.DTOs.Staff;
using Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IStaffService
    {
        Task<Staff> Authenticate(string email, string password);
        Task<List<StaffDTO>> GetAllBaristaAsync();
        Task<bool> CheckEmailExist(string email);
        Task<StaffDTO> GetStaffByIdAsync(int id);
    }
}
