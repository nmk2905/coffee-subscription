using Contracts.DTOs.Customer;
using Contracts.DTOs.Subscription;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Interface;
using System.Numerics;
using System.Security.Claims;


namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly PlanRepository _planRepository;
        public SubscriptionService(SubscriptionRepository subscriptionRepository, PlanRepository planRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }
        public async Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync()
        {
            var result = await _subscriptionRepository.GetAllSubscriptionsAsync();
            
            return result?.Select(subscription => new SubscriptionDTO
            {
                SubscriptionId = subscription.SubscriptionId,
                CustomerId = subscription.Customer?.CustomerId,
                CustomerName = subscription.Customer?.Name,
                PlanId = subscription.Plan?.PlanId,
                PlanName = subscription.Plan?.Name,
                ProductId = subscription.Product?.ProductId,
                ProductName = subscription.Product?.Name,
                ImageUrl = subscription.Product?.ImageUrl,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                RemainingDays = subscription.RemainingDays,
                CreatedAt = subscription.CreatedAt,
                UpdatedAt = subscription.UpdatedAt,
                Status = subscription.Status
            }).ToList() ?? new List<SubscriptionDTO>();
        }

        public async Task<SubscriptionDTO> GetSubscriptionByIdAsync(int id)
        {
            var result = await _subscriptionRepository.GetSubscriptionByIdAsync(id);

            return new SubscriptionDTO
            {
                SubscriptionId = result.SubscriptionId,
                CustomerId = result.Customer?.CustomerId,
                CustomerName = result.Customer?.Name,
                PlanId = result.Plan?.PlanId,
                PlanName = result.Plan?.Name,
                ProductId = result.Product?.ProductId,
                ProductName = result.Product?.Name,
                ImageUrl = result.Product?.ImageUrl,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                RemainingDays = result.RemainingDays,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
                Status = result.Status
            };
        }

        public async Task<List<SubscriptionDTO>> GetMySubs(ClaimsPrincipal user)
        {
            var customerIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                throw new UnauthorizedAccessException("CustomerId not found in token");
            }

            int customerId = int.Parse(customerIdClaim);
            return await GetAllSubscriptionsAsync(customerId);
        }

        public async Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync(int customerId)
        {
            var subs = await _subscriptionRepository.GetAllByCustomerIdAsync(customerId);

            return subs.Select(s => new SubscriptionDTO
            {
                SubscriptionId = s.SubscriptionId,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer?.Name,
                PlanId = s.PlanId,
                PlanName = s.Plan?.Name,
                ProductId = s.ProductId,
                ProductName = s.Product?.Name,
                ImageUrl = s.Product?.ImageUrl,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                RemainingDays = s.RemainingDays,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Status = s.Status
            }).ToList();
        }


        public async Task<SubReponse> Order(OrderDTO request)
        {
            var plan = await _planRepository.GetActivePlanByIdAsync(request.PlanId);

            if (plan == null)
            {
                throw new Exception("Plan not available!");
            }

            var subscription = new Repo.Models.Subscription
            {
                CustomerId = request.CustomerId,
                PlanId = plan.PlanId,
                ProductId = plan.ProductId,
                StartDate = null,   // chưa active
                EndDate = null,     // chưa active
                RemainingDays = 0,  // chưa active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = "PendingPayment"
            };

            await _subscriptionRepository.CreateAsync(subscription);
            await _subscriptionRepository.SaveAsync();

            var createdSubscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscription.SubscriptionId);

            var subs = new SubscriptionDTO
            {
                SubscriptionId = createdSubscription.SubscriptionId,
                CustomerId = createdSubscription.Customer?.CustomerId,
                CustomerName = createdSubscription.Customer?.Name,
                PlanId = createdSubscription.Plan?.PlanId,
                PlanName = createdSubscription.Plan?.Name,
                ProductId = createdSubscription.Product?.ProductId,
                ProductName = createdSubscription.Product?.Name,
                ImageUrl = createdSubscription.Product?.ImageUrl,
                StartDate = createdSubscription.StartDate,
                EndDate = createdSubscription.EndDate,
                RemainingDays = createdSubscription.RemainingDays,
                CreatedAt = createdSubscription.CreatedAt,
                UpdatedAt = createdSubscription.UpdatedAt,
                Status = createdSubscription.Status
            };

            return new SubReponse
            {
                Message = "Order created successfully, waiting for payment.",
                Data = subs
            };
        }

        public async Task<bool> Cancel(int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);

            if (subscription == null)
                throw new Exception("Subscription not found.");

            if (subscription.Status != "PendingPayment")
                throw new Exception("Subscriptions can only be canceled while payment is pending.");

            subscription.Status = "Cancelled";
            subscription.UpdatedAt = DateTime.UtcNow;

            _subscriptionRepository.Update(subscription);
            await _subscriptionRepository.SaveAsync();

            return true;
        }
    }
}
