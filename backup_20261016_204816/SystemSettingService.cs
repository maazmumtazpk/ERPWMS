using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private const string ProtectorPurpose = "ERPWMS.SystemSettings.v1";

        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;

        public SystemSettingService(
            AppDbContext context,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            return await _context.SystemSettings
                .OrderBy(s => s.SettingKey)
                .ToListAsync();
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<SystemSetting?> GetSettingByIdAsync(int settingId)
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingId == settingId);
        }

        // =========================================================
        // GET BY KEY
        // =========================================================
        public async Task<SystemSetting?> GetSettingByKeyAsync(string settingKey)
        {
            if (string.IsNullOrWhiteSpace(settingKey)) return null;

            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == settingKey);
        }

        // =========================================================
        // CREATE
        // =========================================================
        public async Task<SystemSetting> CreateSettingAsync(SystemSetting setting)
        {
            if (setting == null) throw new ArgumentNullException(nameof(setting));

            // Encrypt value if marked as encrypted
            if (setting.IsEncrypted == true && !string.IsNullOrEmpty(setting.SettingValue))
            {
                setting.SettingValue = _protector.Protect(setting.SettingValue);
            }

            setting.UpdatedDate = DateTime.UtcNow;

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            return setting;
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task UpdateSettingAsync(SystemSetting setting)
        {
            if (setting == null) throw new ArgumentNullException(nameof(setting));

            // Encrypt if marked
            if (setting.IsEncrypted == true && !string.IsNullOrEmpty(setting.SettingValue))
            {
                // Only encrypt if not already encrypted
                if (!IsEncrypted(setting.SettingValue))
                {
                    setting.SettingValue = _protector.Protect(setting.SettingValue);
                }
            }

            setting.UpdatedDate = DateTime.UtcNow;

            _context.SystemSettings.Update(setting);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteSettingAsync(int settingId)
        {
            var setting = await GetSettingByIdAsync(settingId);
            if (setting == null) return false;

            _context.SystemSettings.Remove(setting);
            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // GET VALUE
        // =========================================================
        public async Task<string?> GetSettingValueAsync(string settingKey)
        {
            var setting = await GetSettingByKeyAsync(settingKey);
            if (setting == null) return null;

            if (setting.IsEncrypted == true)
            {
                return SafeDecrypt(setting.SettingValue);
            }

            return setting.SettingValue;
        }

        // =========================================================
        // GET TYPED VALUE
        // =========================================================
        public async Task<T?> GetSettingValueAsync<T>(string settingKey, T defaultValue = default!)
        {
            var value = await GetSettingValueAsync(settingKey);
            if (string.IsNullOrEmpty(value)) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        // =========================================================
        // EXISTS
        // =========================================================
        public async Task<bool> SettingExistsAsync(string settingKey, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(settingKey)) return false;

            IQueryable<SystemSetting> query = _context.SystemSettings
                .Where(s => s.SettingKey == settingKey);

            if (excludeId.HasValue)
                query = query.Where(s => s.SettingId != excludeId);

            return await query.AnyAsync();
        }

        // =========================================================
        // ENCRYPTED VALUE
        // =========================================================
        public async Task<string?> GetEncryptedSettingValueAsync(string settingKey)
        {
            var setting = await GetSettingByKeyAsync(settingKey);
            if (setting == null || setting.IsEncrypted != true) return null;

            return SafeDecrypt(setting.SettingValue);
        }

        public async Task SetEncryptedSettingValueAsync(string settingKey, string value)
        {
            var setting = await GetSettingByKeyAsync(settingKey);
            if (setting == null)
                throw new InvalidOperationException($"Setting '{settingKey}' not found.");

            setting.SettingValue = _protector.Protect(value);
            setting.IsEncrypted = true;
            setting.UpdatedDate = DateTime.UtcNow;

            await UpdateSettingAsync(setting);
        }

        // =========================================================
        // AUDIT LOG
        // =========================================================
        public async Task LogSettingChangeAsync(int settingId, string? oldValue, string? newValue, int changedBy)
        {
            var auditLog = new AuditLog
            {
                UserId = changedBy,
                ModuleId = null,
                Action = "SETTINGS_UPDATE",
                RecordId = settingId,
                RecordType = "SystemSetting",
                OldValue = oldValue,
                NewValue = newValue,
                IPAddress = null,
                UserAgent = null,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // HELPERS
        // =========================================================
        private string? SafeDecrypt(string? value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            try
            {
                return _protector.Unprotect(value);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // Legacy value (base64) ya plain text
                try
                {
                    return System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(value));
                }
                catch
                {
                    return value; // Plain text fallback
                }
            }
        }

        private static bool IsEncrypted(string value)
        {
            // DataProtection values start with "CfDJ8" (base64 encoded magic)
            return value.StartsWith("CfDJ8", StringComparison.Ordinal);
        }
    }
}
