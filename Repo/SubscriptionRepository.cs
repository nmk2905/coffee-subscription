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
    public class SubscriptionRepository : GenericRepository<Subscription>
    {
        public SubscriptionRepository() { }

        public Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            return _context.Subscriptions
                .Include(s => s.Customer)
                .Include(s => s.Plan)
                .Include(s => s.Product)
                .ToListAsync();
        }

        public Task<Subscription> GetSubscriptionByIdAsync(int id)
        {
            return _context.Subscriptions
                .Include(s => s.Customer)
                .Include(s => s.Plan)
                .Include(s => s.Product)
                .Include(s => s.Redemptions)
                .FirstOrDefaultAsync(s => s.SubscriptionId == id);
        }

        public async Task<List<Subscription>> GetAllByCustomerIdAsync(int customerId)
        {
            return await _context.Subscriptions
                .Include(s => s.Plan)
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();
        }

        //checkout
        public async Task<List<Subscription>> GetSubscriptionsByPhoneAsync(string phone)
        {
            return await _context.Subscriptions
                .Include(s => s.Plan)
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .Where(s => s.Customer.Phone == phone)
                .ToListAsync();
        }

    }
}
