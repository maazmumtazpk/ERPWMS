
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class SystemSettingService(
        AppDbContext context) : ISystemSettingService
    {
        private readonly AppDbContext _context = context;


        // =========================================================
        // GET ALL SETTINGS
        // =========================================================

        public async Task<IEnumerable<SystemSetting>>
            GetAllSettingsAsync()
        {
            return await _context.SystemSettings
                .OrderBy(s => s.SettingKey)
                .ToListAsync();
        }


        // =========================================================
        // GET SETTING BY ID
        // =========================================================

        public async Task<SystemSetting?>
            GetSettingByIdAsync(
                int settingId)
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(
                    s => s.SettingId == settingId);
        }


        // =========================================================
        // GET SETTING BY KEY
        // =========================================================

        public async Task<SystemSetting?>
            GetSettingByKeyAsync(
                string settingKey)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
            {
                return null;
            }

            return await _context.SystemSettings
                .FirstOrDefaultAsync(
                    s => s.SettingKey == settingKey);
        }


        // =========================================================
        // CREATE SETTING
        // =========================================================

        public async Task<SystemSetting>
            CreateSettingAsync(
                SystemSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(
                    nameof(setting));
            }


            setting.UpdatedDate =
                DateTime.UtcNow;


            _context.SystemSettings.Add(
                setting);

            await _context.SaveChangesAsync();


            return setting;
        }


        // =========================================================
        // UPDATE SETTING
        // =========================================================

        public async Task UpdateSettingAsync(
            SystemSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(
                    nameof(setting));
            }


            setting.UpdatedDate =
                DateTime.UtcNow;


            _context.SystemSettings.Update(
                setting);

            await _context.SaveChangesAsync();
        }


        // =========================================================
        // DELETE SETTING
        // =========================================================

        public async Task<bool>
            DeleteSettingAsync(
                int settingId)
        {
            var setting =
                await GetSettingByIdAsync(
                    settingId);


            if (setting == null)
            {
                return false;
            }


            _context.SystemSettings.Remove(
                setting);

            await _context.SaveChangesAsync();


            return true;
        }


        // =========================================================
        // GET SETTING VALUE
        // =========================================================

        public async Task<string?>
            GetSettingValueAsync(
                string settingKey)
        {
            var setting =
                await GetSettingByKeyAsync(
                    settingKey);


            if (setting == null)
            {
                return null;
            }


            if (setting.IsEncrypted == true)
            {
                return DecryptValue(
                    setting.SettingValue);
            }


            return setting.SettingValue;
        }


        // =========================================================
        // GET TYPED SETTING VALUE
        // =========================================================

        public async Task<T?>
            GetSettingValueAsync<T>(
                string settingKey,
                T defaultValue = default!)
        {
            var value =
                await GetSettingValueAsync(
                    settingKey);


            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }


            try
            {
                return (T)Convert.ChangeType(
                    value,
                    typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }


        // =========================================================
        // SETTING EXISTS
        // =========================================================

        public async Task<bool>
            SettingExistsAsync(
                string settingKey,
                int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
            {
                return false;
            }


            IQueryable<SystemSetting> query =
                _context.SystemSettings
                    .Where(
                        s =>
                            s.SettingKey ==
                            settingKey);


            if (excludeId.HasValue)
            {
                query =
                    query.Where(
                        s =>
                            s.SettingId !=
                            excludeId);
            }


            return await query.AnyAsync();
        }


        // =========================================================
        // GET ENCRYPTED SETTING VALUE
        // =========================================================

        public async Task<string?>
            GetEncryptedSettingValueAsync(
                string settingKey)
        {
            var setting =
                await GetSettingByKeyAsync(
                    settingKey);


            if (setting == null ||
                setting.IsEncrypted != true)
            {
                return null;
            }


            return DecryptValue(
                setting.SettingValue);
        }


        // =========================================================
        // SET ENCRYPTED SETTING VALUE
        // =========================================================

        public async Task SetEncryptedSettingValueAsync(
            string settingKey,
            string value)
        {
            var setting =
                await GetSettingByKeyAsync(
                    settingKey);


            if (setting == null)
            {
                throw new InvalidOperationException(
                    $"Setting '{settingKey}' not found.");
            }


            var encryptedValue =
                EncryptValue(value);


            setting.SettingValue =
                encryptedValue;

            setting.IsEncrypted =
                true;

            setting.UpdatedDate =
                DateTime.UtcNow;


            await UpdateSettingAsync(
                setting);
        }


        // =========================================================
        // LOG SETTING CHANGE
        // =========================================================

        public async Task LogSettingChangeAsync(
            int settingId,
            string? oldValue,
            string? newValue,
            int changedBy)
        {
            var auditLog = new AuditLog
            {
                UserId = changedBy,

                ModuleId = null,

                Action =
                    "SETTINGS_UPDATE",

                RecordId =
                    settingId,

                RecordType =
                    "SystemSetting",

                OldValue =
                    oldValue,

                NewValue =
                    newValue,

                IPAddress =
                    null,

                UserAgent =
                    null,

                Timestamp =
                    DateTime.UtcNow
            };


            _context.AuditLogs.Add(
                auditLog);

            await _context.SaveChangesAsync();
        }


        // =========================================================
        // ENCRYPTION
        // =========================================================
        //
        // NOTE:
        // Base64 is NOT encryption.
        //
        // This is kept compatible with your current implementation.
        // For production, replace this with ASP.NET Core
        // Data Protection / AES / Key Vault.
        // =========================================================

        private static string EncryptValue(
            string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }


            return Convert.ToBase64String(
                global::System.Text.Encoding.UTF8.GetBytes(
                    value));
        }


        // =========================================================
        // DECRYPTION
        // =========================================================

        private static string DecryptValue(
            string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }


            try
            {
                return global::System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(value));
            }
            catch (FormatException)
            {
                // Value may be plain text.
                return value;
            }
        }
    }
}

