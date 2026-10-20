using System;
using System.IO;
using System.Threading.Tasks;
using ERPWMS.Application.Common;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ERPWMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public bool IsTestMode
        {
            get
            {
                var mode = _config["EmailSettings:Mode"] ?? "Test";
                return mode.Equals("Test", StringComparison.OrdinalIgnoreCase);
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (IsTestMode)
                {
                    // TEST MODE: Log to file
                    var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailLogs");
                    Directory.CreateDirectory(logDir);
                    var filePath = Path.Combine(logDir, $"Email_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    await File.WriteAllTextAsync(filePath, $"To: {toEmail}\nSubject: {subject}\n\n{body}");
                    return true;
                }

                // LIVE MODE: Send actual email
                var smtpHost = _config["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderName = _config["EmailSettings:SenderName"];
                var password = _config["EmailSettings:Password"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Email config missing.");
                    return false;
                }

                // Remove spaces from App Password (e.g., 'abcd efgh ijkl mnop' -> 'abcdefghijklmnop')
                password = password.Replace(" ", "");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Subject = subject;

                // Create a nice HTML email for OTP
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 10px; padding: 20px;'>
                        <h2 style='color: #0A1628;'>EnterpriseWMS Password Reset</h2>
                        <p style='color: #475569;'>You requested to reset your password. Use the following OTP:</p>
                        <div style='background: #F8FAFC; border-left: 4px solid #F5A623; padding: 15px; text-align: center; font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #0A1628; margin: 20px 0;'>
                            {body}
                        </div>
                        <p style='color: #475569; font-size: 12px;'>This code will expire in 10 minutes.</p>
                    </div>";

                message.Body = new TextPart("html") { Text = htmlBody };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send error: {ex.Message}");
                return false;
            }
        }
    }
}
