using Microsoft.EntityFrameworkCore;
using Repo.Models;
using Repositories.Base;


namespace Repositories
{
    public class PlanRepository : GenericRepository<SubscriptionPlan>
    {
        public PlanRepository() { }

        public async Task<List<SubscriptionPlan>> GetAllPlansAsync()
        {
            return await _context.SubscriptionPlans
                .Include(p => p.Product).Where(p => p.Active == true)
                .ToListAsync();
        }
        public async Task<SubscriptionPlan> GetPlanByIdAsync(int id)
        {
            return await _context.SubscriptionPlans
                .Include(p => p.Product)
                .FirstOrDefaultAsync(i => i.PlanId == id);
        }

        public async Task<SubscriptionPlan?> GetActivePlanByIdAsync(int planId)
        {
            return await _context.SubscriptionPlans
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PlanId == planId && p.Active == true);
        }
    }
}
