using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repo.Models;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/staffs")]
    public class StaffController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IStaffService _staffService;
        public StaffController(IConfiguration config, IStaffService staffService)
        {
            _config = config;
            _staffService = staffService;
        }

        //JWT
        private string GenerateJSONWebToken(Staff userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userInfo.StaffId.ToString()),
        new Claim(ClaimTypes.Email, userInfo.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, userInfo.Role ?? "Staff")
    };

            var issuer = _config.GetSection("Jwt:ValidIssuers").Get<string[]>()[0];
            var audience = _config.GetSection("Jwt:ValidAudiences").Get<string[]>()[0];

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //GET

        /// <summary>
        /// get-all-barista
        /// </summary>
        /// <returns></returns>
        [HttpGet("baristas")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllBaristaAsync()
        {
            var result = await _staffService.GetAllBaristaAsync();
            return Ok(result);
        }

        /// <summary>
        /// get-barista-by-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Not found!" });
            }
            return Ok(result);
        }

        //POST

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _staffService.Authenticate(request.Email, request.Password);

            if (user == null)
                return Unauthorized();

            var token = GenerateJSONWebToken(user);

            return Ok(token);
        }
    }
}
