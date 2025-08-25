using Contracts.DTOs.checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IRedemptionService
    {
        Task<List<SubscriptionInfoResponse>> GetSubscriptionsByPhoneAsync(string phone);
        Task<CheckoutResponse> RedeemBySubscriptionAsync(CheckoutBySubscriptionRequest request,ClaimsPrincipal user);
    }
}
