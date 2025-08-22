using Contracts.Abstracts.Account;
using Contracts.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repo.Models;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ResetPasswordRequest = Contracts.Abstracts.Account.ResetPasswordRequest;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class customersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHash _passwordHash;
        private readonly ICustomerService _customerService;

        public customersController(IConfiguration config, ICustomerService customerService, IPasswordHash passwordHash)
        {
            _config = config;
            _customerService = customerService;
            _passwordHash = passwordHash;
        }

        //jwt
        private string GenerateJSONWebToken(Customer customerInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var issuer = _config.GetSection("Jwt:ValidIssuers").Get<string[]>()[0];
            var audience = _config.GetSection("Jwt:ValidAudiences").Get<string[]>()[0];

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new Claim[]
                {
            new(ClaimTypes.NameIdentifier, customerInfo.CustomerId.ToString()),
            new(ClaimTypes.Email, customerInfo.Email),
            new Claim(ClaimTypes.Role, "customer")
                },
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //GET

        /// <summary>
        /// get-all-customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomerAsync();
            return Ok(result);
        }

        /// <summary>
        /// get-customer-by-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerService.GetCustomerById(id);
            return Ok(result);
        }

        [HttpGet("my-profile")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> GetMyProfile()
        {
            var customer = await _customerService.GetMyProfile(User);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }
            return Ok(customer);
        }

        //POST

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDTO request)
        {
            var result = await _customerService.RegisterCustomer(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCustomerRequest request)
        {
            var customer = await _customerService.GetCustomerEmail(request.Email);

            if (customer == null)
            {
                return BadRequest("User not found");
            }

            if (customer.IsVerified == false)
            {
                return BadRequest("Please verify your email.");
            }

            var isPasswordValid = _passwordHash.VerifyPasswordHash(request.Password, customer.PasswordHash, customer.PasswordSalt);

            if (!isPasswordValid)
            {
                return BadRequest("Incorrect password.");
            }

            var token = GenerateJSONWebToken(customer);

            return Ok(token);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyTokenRequest request)
        {
            var result = await _customerService.Verify(request.Token);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _customerService.ForgotPassword(email);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var dto = new ResetPasswordDTO
            {
                Token = request.Token,
                Password = request.Password
            };

            var result = await _customerService.ResetPassword(dto);

            return Ok(result);
        }

        /// <summary>
        /// update-profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("my-profile")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var customerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                return Unauthorized("CustomerId not found in token");
            }

            int customerId = int.Parse(customerIdClaim);

            var dto = new UpdateProfileDTO
            {
                CustomerId = customerId,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address
            };

            var response = await _customerService.UpdateProfile(dto);
            return Ok(response);
        }

        
    }
}
