using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISalesOrderStatusRepository
    {
        Task<IEnumerable<SalesOrderStatus>> GetAllAsync(bool includeInactive = false);
        Task<SalesOrderStatus?> GetByIdAsync(int statusId);
        Task<SalesOrderStatus?> GetByNameAsync(string statusName);
        Task<bool> IsActiveAsync(int statusId);
    }
}
