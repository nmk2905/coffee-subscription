using Contracts.Abstracts.Account;
using Contracts.Settings;
using Microsoft.Extensions.Options;
using Service.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        // Create OTP Text
        public string GenerateRandomNumber()
        {
            Random random = new Random();

            string randomo = random.Next(0, 1000000).ToString("D6");

            return randomo;
        }

        public async Task SendEmail(MailRequest mailRequest)
        {
            if (string.IsNullOrWhiteSpace(mailRequest.Email))
            {
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(mailRequest.Email));
            }

            var email = new MimeMessage();

            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.Email));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = mailRequest.EmailBody
            };
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }


        //OPT register
        public async Task SendOtpMail(string name, string otpText, string email)
        {
            var mailRequest = new MailRequest
            {
                Email = email,  // đây chính là To
                Subject = "Thank for registering : OTP",
                EmailBody = GenerateEmailBody(name, otpText)
            };

            await SendEmail(mailRequest);
        }

        private string GenerateEmailBody(string name, string otpText)
        {
            string email = string.Empty;

            email = "<div>";
            email += "<h1> Hi " + name + ", Thanks for registering</h1>";
            email += "<h2>This is your OTP: " + otpText + "</h2>";
            email += "</div>";

            return email;
        }

        //OPT forgot-password
        public async Task SendOtpMailFP(string name, string otpText, string email)
        {
            var mailRequest = new MailRequest
            {
                Email = email,  // đây chính là To
                Subject = "Password Reset OTP",
                EmailBody = EmailBodyFB(name, otpText)
            };

            await SendEmail(mailRequest);
        }

        private string EmailBodyFB(string name, string otpText)
        {
            string email = string.Empty;

            email = "<div>";
            email += "<h1> Hi " + name;
            email += "<h2>Your OTP for resetting your password is: " + otpText + "</h2>";
            email += "</div>";

            return email;
        }
    }
}
