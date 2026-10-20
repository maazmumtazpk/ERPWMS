using System.Threading.Tasks;

namespace ERPWMS.Application.Common
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        bool IsTestMode { get; }
    }
}
