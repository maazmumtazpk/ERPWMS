using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPayrollSettingRepository
    {
        Task<PayrollSetting?> GetSettingAsync();
        Task<PayrollSetting?> GetByIdAsync(int id);
        Task AddAsync(PayrollSetting setting);
        void Update(PayrollSetting setting);
        Task SaveChangesAsync();
    }
}
