
using Contracts.DTOs;
using Contracts.DTOs.Customer;
using Contracts.DTOs.QrCode;
using Microsoft.Identity.Client;
using QRCoder;
using Repo;
using Repo.Models;
using Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _customerRepo;
        private readonly IPasswordHash _passwordHash;
        private readonly IEmailService _emailService;

        public CustomerService(CustomerRepository customerRepo, IPasswordHash passwordHash, IEmailService emailService)
        {
            _customerRepo = customerRepo;
            _passwordHash = passwordHash;
            _emailService = emailService;
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            return await _customerRepo.CheckEmailExist(email);
        }

        public async Task<List<CustomerDTO>> GetAllCustomerAsync()
        {
            var customers = await _customerRepo.GetAllCustomerAsync();

            return customers.Select(c => new CustomerDTO
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                IsVerified = c.IsVerified              
            }).ToList();
        }

        public async Task<Customer> GetCustomerEmail(string email)
        {
            return await _customerRepo.GetCustomerEmail(email);
        }

        public async Task<CustomerDTO> GetCustomerById(int id)
        {
            var customer = await _customerRepo.GetCustomerById(id);

            if (customer == null) return null;

            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                IsVerified = customer.IsVerified
            };
        }

        public async Task<VerifyResponse> Verify(string token)
        {
            var verify = await _customerRepo.Verify(token);

            if (verify == null)
            {
                return new VerifyResponse
                {
                    Success = false,
                    Message = "Invalid or expired token"
                };
            }

            verify.IsVerified = true;
            verify.VerificationToken = null; // xoá token sau khi verify
            await _customerRepo.UpdateAsync(verify);

            return new VerifyResponse
            {
                Success = true,
                Message = "Account verified successfully"
            };
        }

        public async Task<CustomerDTO> RegisterCustomer(RegisterCustomerDTO customer)
        {
            var emailExxits = await _customerRepo.CheckEmailExist(customer.Email);

            if (emailExxits)
            {
                throw new Exception("Email already exists");
            }

            _passwordHash.CreatePasswordHash(customer.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var token = _emailService.GenerateRandomNumber();

            var newCustomer = new Customer
            {
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = token,
                IsVerified = false
            };

            await _emailService.SendOtpMail(customer.Name, token, customer.Email);
            await _customerRepo.CreateAsync(newCustomer);
            await _customerRepo.SaveAsync();

            return new CustomerDTO
            {
                CustomerId = newCustomer.CustomerId,
                Name = newCustomer.Name,
                Email = newCustomer.Email,
                Phone = newCustomer.Phone,
                Address = newCustomer.Address,
                IsVerified = newCustomer.IsVerified
            };
        }

        public async Task<ForgotPasswordReponse> ForgotPassword(string email)
        {
            var cus = await _customerRepo.GetCustomerEmail(email);

            if (cus == null)
            {
                return new ForgotPasswordReponse
                {
                    Success = false,
                    Message = "Not found user"
                };
            }

            var token = _emailService.GenerateRandomNumber();

            await _emailService.SendOtpMailFP(cus.Name, token, cus.Email);

            cus.VerificationToken = token;

            await _customerRepo.UpdateAsync(cus);
            await _customerRepo.SaveAsync();

            return new ForgotPasswordReponse
            {
                Success = true,
                Message = "Please check your email to reset password"
            };
        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordDTO request)
        {
            var cus = await _customerRepo.Verify(request.Token);

            if (cus == null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Invalid or expired token"
                };
            }

            _passwordHash.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            cus.PasswordHash = passwordHash;
            cus.PasswordSalt = passwordSalt;
            cus.VerificationToken = null;

            await _customerRepo.UpdateAsync(cus);

            return new ResetPasswordResponse
            {
                Success = true,
                Message = "Reset Password successfully"
            };
        }

        public async Task<UpdateProfileResponse> UpdateProfile(UpdateProfileDTO request)
        {
            var customerExist = await _customerRepo.GetCustomerById(request.CustomerId);
            if (customerExist == null)
            {
                throw new Exception("Customer not found");
            }

            if (!string.IsNullOrEmpty(request.Name))
                customerExist.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Phone))
                customerExist.Phone = request.Phone;

            if (!string.IsNullOrEmpty(request.Address))
                customerExist.Address = request.Address;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != customerExist.Email)
            {
                var token = _emailService.GenerateRandomNumber();

                await _emailService.SendOtpMail(
                    request.Name ?? customerExist.Name,
                    token,
                    request.Email
                );

                customerExist.VerificationToken = token;
                customerExist.IsVerified = false;
                customerExist.Email = request.Email;
            }

            await _customerRepo.UpdateAsync(customerExist);
            await _customerRepo.SaveAsync();

            var response = new UpdateProfileResponse
            {
                Message = "Update profile successfully",
                Data = new UpdateProfileDTO
                {
                    CustomerId = customerExist.CustomerId,
                    Name = customerExist.Name,
                    Email = customerExist.Email,
                    Phone = customerExist.Phone,
                    Address = customerExist.Address
                }
            };

            return response;
        }

        public async Task<CustomerDTO> GetMyProfile(ClaimsPrincipal user)
        {
            var customerIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                throw new UnauthorizedAccessException("CustomerId not found in token");
            }

            int customerId = int.Parse(customerIdClaim);
            return await GetCustomerById(customerId);
        }

        public async Task<byte[]> GenerateCustomerQrAsync(ClaimsPrincipal user)
        {
            var name = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var phone = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;

            if (string.IsNullOrEmpty(phone))
                throw new Exception("Phone number not found in JWT");

            var payload = new { Name = name, Phone = phone };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(json, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20); // return byte[] (PNG)
        }

        public async Task<Customer> GetOrCreateGoogleCustomerAsync(string email, string name)
        {
            // 1) Tìm sẵn theo email
            var existing = await _customerRepo.GetCustomerEmail(email);
            if (existing != null)
            {
                // Nếu đang chưa verify thì bật verified để login được
                if (existing.IsVerified != true)
                {
                    existing.IsVerified = true;
                    existing.VerificationToken = null;
                    await _customerRepo.UpdateAsync(existing);
                    await _customerRepo.SaveAsync();
                }
                return existing;
            }

            // 2) Tạo tài khoản mới (password ngẫu nhiên vì schema cần hash)
            _passwordHash.CreatePasswordHash(Guid.NewGuid().ToString("N"),
                out byte[] hash, out byte[] salt);

            var newCustomer = new Customer
            {
                Name = name ?? "",
                Email = email,
                Phone = "",
                Address = "",
                PasswordHash = hash,
                PasswordSalt = salt,
                IsVerified = true,          // <- rất quan trọng
                VerificationToken = null
            };

            await _customerRepo.CreateAsync(newCustomer);
            await _customerRepo.SaveAsync();
            return newCustomer;
        }

    }
}
