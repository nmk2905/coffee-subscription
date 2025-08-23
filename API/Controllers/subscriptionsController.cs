using Contracts.DTOs.Payment;
using Contracts.DTOs.SepayWebhook;
using Contracts.DTOs.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;
using System.Security.Claims;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class subscriptionsController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        public subscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // GET

        /// <summary>
        /// get-all-subscriptions - role admin only
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllSubs()
        {
            var plans = await _subscriptionService.GetAllSubscriptionsAsync();
            return Ok(plans);
        }

        /// <summary>
        /// get-subription-by-id - role admin only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSubByIdAsync(int id)
        {
            var plan = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (plan == null)
            {
                return NotFound(new { message = "No subscriptions found!" });
            }
            return Ok(plan);
        }

        /// <summary>
        /// role customer only, get my subscriptions
        /// </summary>
        /// <returns></returns>
        [HttpGet("my-subscriptions")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> GetMySubs()
        {
            var customer = await _subscriptionService.GetMySubs(User);
            if (customer == null)
            {
                return NotFound("Subscriptions not found");
            }
            return Ok(customer);
        }

        // POST

        /// <summary>
        /// order-subscription - role customer only
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Order([FromBody] OrderRequest request)
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Customer not found!"));
            var dto = new OrderDTO { CustomerId = customerId, PlanId = request.PlanId };
            var result = await _subscriptionService.Order(dto);
            return Ok(result);
        }


        [HttpPost("payment-callback")]
        [AllowAnonymous] // Sepay sẽ gọi webhook, không có JWT
        public async Task<IActionResult> PaymentCallback([FromBody] SepayWebhookRequest request)
        {
            await _subscriptionService.HandleSepayWebhookAsync(request);
            return Ok(new { success = true });
        }


        //DELETE

        /// <summary>
        /// cacnel-subscription, -role customer, allow cancel only if subscription status is PendingPayment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _subscriptionService.Cancel(id);
                if (!result)
                    return BadRequest(new { message = "Failed to cancel subscription." });

                return Ok(new { message = "Subscription has been successfully cancelled." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
