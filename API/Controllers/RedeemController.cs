using Contracts.DTOs.checkout;
using Contracts.DTOs.QrCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/redeems")]
    public class RedeemController : Controller
    {
        private readonly IRedemptionService _redemptionService;
        public RedeemController(IRedemptionService redemptionService)
        {
            _redemptionService = redemptionService;
        }

        /// <summary>
        /// staff scan qr gui sđt lay subs cua customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // B1: Staff quét QR, gửi phone => lấy tất cả subscriptions
        [HttpPost("scan-qr")]
        public async Task<IActionResult> ScanQr([FromBody] ScanQrRequest request)
        {
            var subs = await _redemptionService.GetSubscriptionsByPhoneAsync(request.Phone);
            if (subs == null || !subs.Any())
                return NotFound("No subscriptions found for this customer.");
            return Ok(subs);
        }

        /// <summary>
        /// staff checkout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // B2: Staff chọn subscription để checkout
        [HttpPost("redeem")]
        [Authorize(Roles ="barista")] 
        public async Task<IActionResult> Redeem([FromBody] CheckoutBySubscriptionRequest request)
        {
            var result = await _redemptionService.RedeemBySubscriptionAsync(request, User);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
