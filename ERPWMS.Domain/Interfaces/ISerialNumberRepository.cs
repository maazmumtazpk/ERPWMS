using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISerialNumberRepository
    {
        Task<IEnumerable<SerialNumber>> GetByProductIdAsync(int productId);
        Task<IEnumerable<SerialNumber>> GetByGrndetailIdAsync(int grnDetailId);
        Task<IEnumerable<SerialNumber>> GetBySalesDetailIdAsync(int salesDetailId);
        Task<IEnumerable<SerialNumber>> GetByCustomerIdAsync(int customerId);
        Task<SerialNumber?> GetBySerialNoAsync(string serialNo);
        Task<IEnumerable<SerialNumber>> GetByStatusAsync(string status);
        Task<IEnumerable<SerialNumber>> GetAvailableAsync(int productId);
        Task<bool> IsSerialNoExistsAsync(string serialNo);
        Task AddAsync(SerialNumber entity);
        Task AddRangeAsync(IEnumerable<SerialNumber> entities);
        Task UpdateAsync(SerialNumber entity);
        Task UpdateRangeAsync(IEnumerable<SerialNumber> entities);
        Task<bool> CanBeDeletedAsync(int serialId);
        Task UpdateStatusAsync(int serialId, string status);
        Task AssignToSalesDetailAsync(int serialId, int salesDetailId, int customerId, DateTime outDate);
    }
}
