using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Abstracts.Account;

namespace Service.Interface
{
    public interface IEmailService
    {
        Task SendEmail(MailRequest mailRequest);
        //register
        Task SendOtpMail(string name, string otpText, string email);
        //forgot password
        Task SendOtpMailFP(string name, string otpText, string email);
        string GenerateRandomNumber();
    }
}
