
using Contracts.DTOs;
using Contracts.DTOs.Customer;
using Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerEmail(string email);
        Task<List<CustomerDTO>> GetAllCustomerAsync();
        Task<bool> CheckEmailExist(string email);
        Task<CustomerDTO> GetCustomerById(int id);
        Task<VerifyResponse> Verify(string token);
        Task<CustomerDTO> RegisterCustomer(RegisterCustomerDTO customer);
        Task<ForgotPasswordReponse> ForgotPassword(string email);
        Task<ResetPasswordResponse> ResetPassword(ResetPasswordDTO request);
        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileDTO request);
        Task<CustomerDTO> GetMyProfile(ClaimsPrincipal user);
    }
}
