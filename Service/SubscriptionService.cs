using Contracts.DTOs.Customer;
using Contracts.DTOs.Payment;
using Contracts.DTOs.SepayWebhook;
using Contracts.DTOs.Subscription;
using Microsoft.EntityFrameworkCore;
using Repo.Models;
using Repositories;
using Services.Interface;
using System.Numerics;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly PlanRepository _planRepository;
        private readonly PaymentRepository _paymentRepo;
        public SubscriptionService(SubscriptionRepository subscriptionRepository,
            PlanRepository planRepository, PaymentRepository paymentRepo)
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _paymentRepo = paymentRepo;
        }


        //
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

        //my-subs
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

        //
        public async Task<SubResponse> Order(OrderDTO request)
        {
            var plan = await _planRepository.GetActivePlanByIdAsync(request.PlanId);
            if (plan == null) throw new Exception("Plan not available!");

            var subscription = new Subscription
            {
                CustomerId = request.CustomerId,
                PlanId = plan.PlanId,
                ProductId = plan.ProductId,
                Status = "PendingPayment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _subscriptionRepository.CreateAsync(subscription);
            await _subscriptionRepository.SaveAsync();

            var payment = new Payment
            {
                SubscriptionId = subscription.SubscriptionId,
                Amount = plan.Price,
                Method = "BankTransfer",
                Status = "Pending"
            };
            await _paymentRepo.CreateAsync(payment);
            await _paymentRepo.SaveAsync();

            var transferContent = $"SEVQR SUB{subscription.SubscriptionId}";

            // Generate VietQR link từ SePay
            var qrUrl = $"https://qr.sepay.vn/img?bank=Vietinbank&acc=105874237719&amount={plan.Price}&des={transferContent}";

            var subs = await _subscriptionRepository.GetSubscriptionByIdAsync(subscription.SubscriptionId);
            var subsDto = new SubscriptionDTO
            {
                SubscriptionId = subs.SubscriptionId,
                CustomerId = subs.CustomerId,
                CustomerName = subs.Customer?.Name,
                PlanId = subs.PlanId,
                PlanName = subs.Plan?.Name,
                ProductId = subs.ProductId,
                ProductName = subs.Product?.Name,
                ImageUrl = subs.Product?.ImageUrl,
                Status = subs.Status
            };

            return new SubResponse
            {
                Message = "Order created successfully, please scan QR to pay.",
                Data = subsDto,
                QrUrl = qrUrl,
                BankAccount = "105874237719",
                BankName = "Vietinbank",
                AccountHolder = "NGUYEN MINH KHOI",
                TransferContent = transferContent,
                Amount = plan.Price
            };
        }

        public async Task HandleSepayWebhookAsync(SepayWebhookRequest request)
        {
            // chỉ xử lý giao dịch tiền vào
            if (request.TransferType != "in") return;

            var match = Regex.Match(request.Content ?? "", @"SEVQR\s+SUB(\d+)");

            if (!match.Success) return;

            var subId = int.Parse(match.Groups[1].Value);
            // 🔑 Load luôn Subscription kèm Plan/Product
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subId);
            if (subscription == null) return;

            var payment = await _paymentRepo.GetBySubscriptionIdAsync(subId);
            if (payment == null) return;

            // 🔑 Nên so sánh <= hoặc Math.Abs để tránh lệch nhỏ
            if (Math.Abs(payment.Amount - request.TransferAmount) <= 1)
            {
                payment.Status = "Success";
                payment.PaidAt = DateTime.UtcNow;
                await _paymentRepo.UpdateAsync(payment);
                await _paymentRepo.SaveAsync();

                subscription.Status = "Active";
                subscription.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
                subscription.EndDate = DateOnly.FromDateTime(
                    DateTime.UtcNow.AddDays(subscription.Plan.DurationDays)
                );
                subscription.RemainingDays = subscription.Plan.DurationDays;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateAsync(subscription);
                await _subscriptionRepository.SaveAsync();
            }
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
