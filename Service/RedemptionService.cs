using Contracts.Abstracts.Account;
using Contracts.DTOs.checkout;
using Repo.Models;
using Repositories;
using Service.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class RedemptionService : IRedemptionService
    {
        private readonly NotiRepository _notiRepo;
        private readonly RedemptionRepository _redemptionRepo;
        private readonly SubscriptionRepository _subscriptionrepo; 
        private readonly IEmailService _emailService;
        public RedemptionService(SubscriptionRepository subscriptionService,
            IEmailService emailService, RedemptionRepository redemptionRepo, NotiRepository notiRepo)
        {
            _subscriptionrepo = subscriptionService;
            _emailService = emailService;
            _redemptionRepo = redemptionRepo;
            _notiRepo = notiRepo;
        }

        // B1: staff quét QR, lấy phone => trả về subscriptions
        public async Task<List<SubscriptionInfoResponse>> GetSubscriptionsByPhoneAsync(string phone)
        {
            var subs = await _subscriptionrepo.GetSubscriptionsByPhoneAsync(phone);
            return subs.Select(s => new SubscriptionInfoResponse
            {
                SubscriptionId = s.SubscriptionId,
                PlanName = s.Plan.Name,
                EndDate = s.EndDate,
                RemainingDays = s.RemainingDays,
                Status = s.Status,
                ProductName = s.Product?.Name
            }).ToList();
        }

        // B2: staff chọn subscription => redeem
        public async Task<CheckoutResponse> RedeemBySubscriptionAsync(CheckoutBySubscriptionRequest request,ClaimsPrincipal user)
        {
            // 0) Validate input
            if (request.Quantity <= 0)
                return new CheckoutResponse { Success = false, Message = "Quantity must be greater than 0" };

            // 1) Lấy subscription + include Plan/Product/Customer
            // Đảm bảo repo đúng tên/hàm:
            var sub = await _subscriptionrepo.GetSubscriptionByIdAsync(request.SubscriptionId);
            if (sub == null)
                return new CheckoutResponse { Success = false, Message = "Subscription not found" };

            // 2) Validate trạng thái + ngày bắt đầu/kết thúc
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (sub.Status != "Active")
                return new CheckoutResponse { Success = false, Message = "Subscription is not active" };

            if (sub.StartDate != null && sub.StartDate > today)
                return new CheckoutResponse { Success = false, Message = "Subscription not started yet" };

            if (sub.EndDate != null && sub.EndDate < today)
                return new CheckoutResponse { Success = false, Message = "Subscription expired" };

            // 3) Check MaxPerVisit (nếu có cấu hình)
            if (sub.Plan?.MaxPerVisit is int maxPerVisit && maxPerVisit > 0 && request.Quantity > maxPerVisit)
                return new CheckoutResponse { Success = false, Message = "Exceed max per visit quota" };

            // 4) Check DailyQuota (nếu có cấu hình)
            if (sub.Plan?.DailyQuota is int dailyQuota && dailyQuota > 0)
            {
                var redeemedToday = await _redemptionRepo.GetTotalRedeemedTodayAsync(sub.SubscriptionId);
                if (redeemedToday + request.Quantity > dailyQuota)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Message = $"Daily quota exceeded. Already redeemed {redeemedToday}/{dailyQuota} today."
                    };
                }
            }

            // 5) Lấy staffId từ JWT (bắt buộc)
            var staffIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(staffIdClaim, out var staffId) || staffId <= 0)
                return new CheckoutResponse { Success = false, Message = "Staff identity is missing or invalid" };

            // 6) Gắn storeId cứng và productId từ subscription (bắt buộc có productId)
            if (sub.ProductId == null)
                return new CheckoutResponse { Success = false, Message = "Subscription does not specify a product" };

            var redemption = new Redemption
            {
                SubscriptionId = sub.SubscriptionId,
                StoreId = 1, // cứng theo yêu cầu
                StaffId = staffId,
                ProductId = sub.ProductId.Value,
                Quantity = request.Quantity,
                RedeemedAt = DateTime.UtcNow
            };

            await _redemptionRepo.CreateAsync(redemption);

            // 7) Gửi email (không làm fail checkout nếu email lỗi)
            try
            {
                var recipient = sub.Customer?.Email;
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    await _emailService.SendEmail(new MailRequest
                    {
                        Email = recipient,
                        Subject = "Checkout thành công",
                        EmailBody = $"Bạn vừa nhận {request.Quantity}x {sub.Product?.Name} thành công."
                    });
                }
            }
            catch
            {
                // log nếu cần, không throw
            }

            // 8) Tạo notification
            await _notiRepo.CreateAsync(new Notification
            {
                CustomerId = sub.CustomerId,
                Title = "Checkout thành công",
                Body = $"Bạn đã nhận {request.Quantity}x {sub.Product?.Name}",
                Type = "Checkout",
                CreatedAt = DateTime.UtcNow,
                Status = "Unread"
            });

            // 9) Trả response
            return new CheckoutResponse
            {
                Success = true,
                Message = "Checkout successful",
                RedeemedAt = redemption.RedeemedAt!.Value,
                PlanName = sub.Plan?.Name,
                ProductName = sub.Product?.Name,
                Quantity = request.Quantity
            };
        }


    }
}
