using Contracts.DTOs;
using Contracts.DTOs.SubscriptionPlan;
using Repo.Models;
using Repositories;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PlanService : IPlanService
    {
        private readonly PlanRepository _planRepository;
        public PlanService(PlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<List<PlanDTO>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllPlansAsync();

            return plans?.Select(plan => new PlanDTO
            {
                PlanId = plan.PlanId,
                Name = plan.Name,
                Description = plan.Description,
                ProductName = plan.Product?.Name,
                ImageUrl = plan.Product?.ImageUrl,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                DailyQuota = plan.DailyQuota,
                MaxPerVisit = plan.MaxPerVisit,
                Active = plan.Active
            }).ToList() ?? new List<PlanDTO>();
        }

        public async Task<PlanDTO> GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);

            return new PlanDTO
            {
                PlanId = plan.PlanId,
                Name = plan.Name,
                Description = plan.Description,
                ProductName = plan.Product?.Name,
                ImageUrl = plan.Product?.ImageUrl,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                DailyQuota = plan.DailyQuota,
                MaxPerVisit = plan.MaxPerVisit,
                Active = plan.Active
            };
        }

        public async Task<PlanReponse> AddPlan(AddPlanDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Plan name is required");

            if (request.Price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            if (request.DurationDays <= 0)
                throw new ArgumentException("DurationDays must be greater than 0");

            var newPlan = new SubscriptionPlan
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

            await _planRepository.CreateAsync(newPlan);
            await _planRepository.SaveAsync();

            var createdPlan = await _planRepository.GetPlanByIdAsync(newPlan.PlanId);

            var planDto = new PlanDTO
            {
                PlanId = createdPlan.PlanId,
                Name = createdPlan.Name,
                Description = createdPlan.Description,
                ProductName = createdPlan.Product?.Name,
                ImageUrl = createdPlan.Product?.ImageUrl,
                Price = createdPlan.Price,
                DurationDays = createdPlan.DurationDays,
                DailyQuota = createdPlan.DailyQuota,
                MaxPerVisit = createdPlan.MaxPerVisit,
                Active = createdPlan.Active
            };

            return new PlanReponse
            {
                Message = "Plan created successfully",
                Data = planDto
            };
        }

        public async Task<PlanReponse> UpdatePlan(UpdatePlanDTO request)
        {
            var plan = await _planRepository.GetPlanByIdAsync(request.PlanId);

            if (plan == null)
            {
                throw new Exception("PLan not found.");
            }

            //validation
            if (!string.IsNullOrWhiteSpace(request.Name))
                plan.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                plan.Description = request.Description;

            if (request.ProductId.HasValue)
                plan.ProductId = request.ProductId.Value;

            if (request.Price.HasValue && request.Price.Value > 0)
                plan.Price = request.Price.Value;

            if (request.DurationDays.HasValue && request.DurationDays.Value > 0)
                plan.DurationDays = request.DurationDays.Value;

            if (request.DailyQuota.HasValue && request.DailyQuota.Value > 0)
                plan.DailyQuota = request.DailyQuota.Value;

            if (request.MaxPerVisit.HasValue && request.MaxPerVisit.Value > 0)
                plan.MaxPerVisit = request.MaxPerVisit.Value;

            if (request.Active.HasValue)
                plan.Active = request.Active.Value;


            await _planRepository.UpdateAsync(plan);
            await _planRepository.SaveAsync();

            var updatedPlan = await _planRepository.GetPlanByIdAsync(request.PlanId);

            var planDto = new PlanDTO
            {
                PlanId = updatedPlan.PlanId,
                Name = updatedPlan.Name,
                Description = updatedPlan.Description,
                ProductName = updatedPlan.Product?.Name,
                ImageUrl = updatedPlan.Product?.ImageUrl,
                Price = updatedPlan.Price,
                DurationDays = updatedPlan.DurationDays,
                DailyQuota = updatedPlan.DailyQuota,
                MaxPerVisit = updatedPlan.MaxPerVisit,
                Active = updatedPlan.Active
            };

            return new PlanReponse
            {
                Message = "Plan updated successfully",
                Data = planDto
            };
        }

        public async Task<ForgotPasswordReponse> DeactivatePlan(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);

            if (plan == null)
            {
                return new ForgotPasswordReponse
                {
                    Success = false,
                    Message = "Plan not found"
                };
            }

            plan.Active = false;

            await _planRepository.UpdateAsync(plan);
            await _planRepository.SaveAsync();

            return new ForgotPasswordReponse
            {
                Success = true,
                Message = $"Plan with ID {id} has been deactivated successfully."
            };
        }
    }
}
