using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ISystemSettingService
    {
        // Setting management
        Task<IEnumerable<SystemSetting>> GetAllSettingsAsync();
        Task<SystemSetting?> GetSettingByIdAsync(int settingId);
        Task<SystemSetting?> GetSettingByKeyAsync(string settingKey);
        Task<SystemSetting> CreateSettingAsync(SystemSetting setting);
        Task UpdateSettingAsync(SystemSetting setting);
        Task<bool> DeleteSettingAsync(int settingId);

        // Setting value operations
        Task<string?> GetSettingValueAsync(string settingKey);
        Task<T?> GetSettingValueAsync<T>(string settingKey, T defaultValue = default!);
        Task<bool> SettingExistsAsync(string settingKey, int? excludeId = null);

        // Encrypted settings
        Task<string?> GetEncryptedSettingValueAsync(string settingKey);
        Task SetEncryptedSettingValueAsync(string settingKey, string value);

        // Audit
        Task LogSettingChangeAsync(int settingId, string? oldValue, string? newValue, int changedBy);
    }
}