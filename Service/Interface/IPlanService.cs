

using Contracts.DTOs;
using Contracts.DTOs.Product;
using Contracts.DTOs.SubscriptionPlan;
using Repo.Models;

namespace Services.Interface
{
    public interface IPlanService
    {
        Task<List<PlanDTO>> GetAllPlansAsync();
        Task<PlanDTO> GetPlanByIdAsync(int id);
        Task<PlanReponse> AddPlan(AddPlanDTO request);
        Task<PlanReponse> UpdatePlan(UpdatePlanDTO request);
        Task<ForgotPasswordReponse> UnActivePlan(int id);
    }
}
