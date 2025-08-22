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
    public class plansController : Controller
    {
        private readonly IPlanService _planService;
        public plansController(IPlanService planService)
        {
            _planService = planService;
        }

        // GET

        /// <summary>
        /// get-all-plans
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPlansAsync()
        {
            var plans = await _planService.GetAllPlansAsync();
            return Ok(plans);
        }

        /// <summary>
        /// get-plan-by-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
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

        /// <summary>
        /// add-plan
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
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

        /// <summary>
        /// update-plan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
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

        /// <summary>
        /// unactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeactivatePlan(int id)
        {
            var result = await _planService.DeactivatePlan(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

    }
}
