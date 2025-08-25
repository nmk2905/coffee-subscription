using Contracts.DTOs.Payment;
using Contracts.DTOs.SepayWebhook;
using Contracts.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repositories;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly SubscriptionRepository _subscriptionRepository;
        public PaymentService(PaymentRepository paymentRepo, SubscriptionRepository subscriptionRepository)
        {
            _paymentRepo = paymentRepo;
            _subscriptionRepository = subscriptionRepository;
        }

    }
}
