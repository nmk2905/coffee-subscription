using Contracts.DTOs.Staff;
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
    public class StaffService : IStaffService
    {
        private readonly StaffRepository _staffRepo;
        public StaffService(StaffRepository staffRepo)
        {
            _staffRepo = staffRepo;
        }
        public async Task<bool> CheckEmailExist(string email)
        {
            return await _staffRepo.CheckEmailExist(email);
        }

        public async Task<List<StaffDTO>> GetAllBaristaAsync()
        {
            var staff = await _staffRepo.GetAllBaristaAsync();

            return staff?.Select(s => new StaffDTO
            {
                StaffId = s.StaffId,
                Name = s.Name,
                StoreName = s.Store?.Name,
                Role = s.Role,
                Email = s.Email,
                Phone = s.Phone
            }).ToList() ?? new List<StaffDTO>();
        }

        public Task<Staff> Authenticate(string email, string password)
        {
            return _staffRepo.GetStaffAccount(email, password);
        }

        public async Task<StaffDTO> GetStaffByIdAsync(int id)
        {
            var staff = await _staffRepo.GetBaristaById(id);

            return new StaffDTO
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                StoreName = staff.Store?.Name,
                Role = staff.Role,
                Email = staff.Email,
                Phone = staff.Phone
            };
        }
    }
}
