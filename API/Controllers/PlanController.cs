using Contracts.DTOs;
using Contracts.DTOs.SubscriptionPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        // GET

        [HttpGet("get-all-plans")]
        public async Task<IActionResult> GetAllPlansAsync()
        {
            var plans = await _planService.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpGet("get-plan-by-id/{id}")]
        public async Task<IActionResult> GetPlanByIdAsync(int id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound(new { message = "No plans found!" });
            }
            return Ok(plan);
        }

        // POST

        [HttpPost("add-plan")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPlan([FromBody] AddPlanRequest request)
        {
            var dto = new AddPlanDTO
            {
                Name = request.Name,
                Description = request.Description,
                ProductId = request.ProductId,
                Price = request.Price,
                DurationDays = request.DurationDays,
                DailyQuota = request.DailyQuota,
                MaxPerVisit = request.MaxPerVisit,
                Active = request.Active
            };

            var result = await _planService.AddPlan(dto);

            return Ok(result);
        }

        // PUT

        [HttpPut("update-plan")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] UpdatePlanRequest request)
        {
            var dto = new UpdatePlanDTO
            {
                PlanId = id,
                Name = request.Name,
                Description = request.Description,
                ProductId = request.ProductId,
                Price = request.Price,
                DurationDays = request.DurationDays,
                DailyQuota = request.DailyQuota,
                MaxPerVisit = request.MaxPerVisit,
                Active = request.Active
            };
            var result = await _planService.UpdatePlan(dto);

            return Ok(result);
        }

        [HttpPut("unactive/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnActivePlan(int id)
        {
            var result = await _planService.UnActivePlan(id);

            if (!result.Success)
            {
                return NotFound(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Ok(new
            {
                success = true,
                message = result.Message
            });
        }

    }
}
