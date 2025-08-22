using Contracts.DTOs.Customer;
using Contracts.DTOs.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISubscriptionService 
    {
        Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync();
        Task<SubscriptionDTO> GetSubscriptionByIdAsync(int id);
        Task<List<SubscriptionDTO>> GetMySubs(ClaimsPrincipal user);
        Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync(int customerId);
        Task<SubReponse> Order(OrderDTO request);
        Task<bool> Cancel(int subscriptionId);
    }
}
