using Microsoft.EntityFrameworkCore;
using Repo.Models;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class RedemptionRepository : GenericRepository<Redemption>
    {
        public RedemptionRepository() { }
        public async Task<int> GetTotalRedeemedTodayAsync(int subscriptionId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Redemptions
                .Where(r => r.SubscriptionId == subscriptionId
                         && r.RedeemedAt.HasValue
                         && r.RedeemedAt.Value.Date == today)
                .SumAsync(r => r.Quantity ?? 0);
        }
    }
}
