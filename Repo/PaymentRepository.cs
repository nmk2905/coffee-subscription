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
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository() { }

        public async Task<Payment?> GetByTransferContentAsync(string transferContent)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(p => p.TransferContent == transferContent);
        }

        public async Task<Payment> GetBySubscriptionIdAsync(int subscriptionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.SubscriptionId == subscriptionId);
        }


        //public async Task<Payment> GetByTransactionCodeAsync(string code)
        //{
        //    var payment = await _context.Payments
        //        .Include(p => p.Subscription)
        //        .ThenInclude(s => s.Plan)
        //        .FirstOrDefaultAsync(p => p.Method == code);
        //    if (payment != null)
        //    {
        //        _context.Entry(payment).State = EntityState.Detached;
        //    }
        //    return payment;
        //}
    }
}
