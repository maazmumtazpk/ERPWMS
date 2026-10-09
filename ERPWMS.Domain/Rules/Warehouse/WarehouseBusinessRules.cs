using ERPWMS.Domain.Exceptions.Warehouse;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// Model aliases to avoid namespace/type name conflicts
using WarehouseModel = ERPWMS.Database.Models.Warehouse;
using ZoneModel = ERPWMS.Database.Models.Zone;
using RackModel = ERPWMS.Database.Models.Rack;
using BinModel = ERPWMS.Database.Models.Bin;

namespace ERPWMS.Domain.Rules.Warehouse
{
    public static class WarehouseBusinessRules
    {
        // =============================================================
        // WAREHOUSE RULES
        // =============================================================

        public static void ValidateWarehouseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WarehouseDomainValidationException(
                    "Warehouse name is required.");
            }

            if (name.Length > 200)
            {
                throw new WarehouseDomainValidationException(
                    "Warehouse name cannot exceed 200 characters.");
            }
        }

        public static void ValidateWarehouseCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new WarehouseDomainValidationException(
                    "Warehouse code is required.");
            }

            if (code.Length > 20)
            {
                throw new WarehouseDomainValidationException(
                    "Warehouse code cannot exceed 20 characters.");
            }

            if (!Regex.IsMatch(code, @"^[A-Z0-9\-_]+$"))
            {
                throw new WarehouseDomainValidationException(
                    "Warehouse code must contain only uppercase letters, numbers, hyphens, and underscores.");
            }
        }

        public static void ValidateWarehouseEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            try
            {
                var address = new MailAddress(email);

                if (!string.Equals(
                        address.Address,
                        email,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new WarehouseDomainValidationException(
                        "Invalid email format.");
                }
            }
            catch (WarehouseDomainValidationException)
            {
                throw;
            }
            catch
            {
                throw new WarehouseDomainValidationException(
                    "Invalid email format.");
            }
        }

        public static void ValidateWarehousePhone(string? phone)
        {
            if (!string.IsNullOrWhiteSpace(phone) &&
                phone.Length > 20)
            {
                throw new WarehouseDomainValidationException(
                    "Phone number cannot exceed 20 characters.");
            }
        }

        public static void ValidateWarehouseAddress(string? address)
        {
            if (!string.IsNullOrWhiteSpace(address) &&
                address.Length > 500)
            {
                throw new WarehouseDomainValidationException(
                    "Address cannot exceed 500 characters.");
            }
        }

        public static async Task ValidateWarehouseCodeUniquenessAsync(
            IWarehouseRepository repository,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await repository.ExistsByCodeAsync(
                code,
                excludeId,
                cancellationToken);

            if (exists)
            {
                throw new WarehouseDomainValidationException(
                    $"Warehouse code '{code}' already exists.");
            }
        }

        public static void ValidateWarehouseForActivation(
            WarehouseModel warehouse)
        {
            if (warehouse.IsDeleted == true)
            {
                throw new WarehouseDomainValidationException(
                    "Cannot activate a deleted warehouse.");
            }
        }

        public static async Task ValidateWarehouseForDeactivationAsync(
            IWarehouseRepository repository,
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            var hasActiveZones =
                await repository.HasActiveZonesAsync(
                    warehouseId,
                    cancellationToken);

            if (hasActiveZones)
            {
                throw new WarehouseHasActiveZonesException(
                    warehouseId);
            }
        }


        // =============================================================
        // ZONE RULES
        // =============================================================

        public static void ValidateZoneName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WarehouseDomainValidationException(
                    "Zone name is required.");
            }

            if (name.Length > 100)
            {
                throw new WarehouseDomainValidationException(
                    "Zone name cannot exceed 100 characters.");
            }
        }

        public static void ValidateZoneCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new WarehouseDomainValidationException(
                    "Zone code is required.");
            }

            if (code.Length > 20)
            {
                throw new WarehouseDomainValidationException(
                    "Zone code cannot exceed 20 characters.");
            }

            if (!Regex.IsMatch(code, @"^[A-Z0-9\-_]+$"))
            {
                throw new WarehouseDomainValidationException(
                    "Zone code must contain only uppercase letters, numbers, hyphens, and underscores.");
            }
        }

        public static async Task ValidateZoneCodeUniquenessAsync(
            IWarehouseRepository repository,
            int warehouseId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await repository.ZoneExistsByCodeAsync(
                warehouseId,
                code,
                excludeId,
                cancellationToken);

            if (exists)
            {
                throw new WarehouseDomainValidationException(
                    $"Zone code '{code}' already exists in this warehouse.");
            }
        }

        public static void ValidateZoneForActivation(
            ZoneModel zone)
        {
            if (zone.IsDeleted == true)
            {
                throw new WarehouseDomainValidationException(
                    "Cannot activate a deleted zone.");
            }
        }


        // =============================================================
        // RACK RULES
        // =============================================================

        public static void ValidateRackName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WarehouseDomainValidationException(
                    "Rack name is required.");
            }

            if (name.Length > 100)
            {
                throw new WarehouseDomainValidationException(
                    "Rack name cannot exceed 100 characters.");
            }
        }

        public static void ValidateRackCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new WarehouseDomainValidationException(
                    "Rack code is required.");
            }

            if (code.Length > 20)
            {
                throw new WarehouseDomainValidationException(
                    "Rack code cannot exceed 20 characters.");
            }

            if (!Regex.IsMatch(code, @"^[A-Z0-9\-_]+$"))
            {
                throw new WarehouseDomainValidationException(
                    "Rack code must contain only uppercase letters, numbers, hyphens, and underscores.");
            }
        }

        public static async Task ValidateRackCodeUniquenessAsync(
            IWarehouseRepository repository,
            int zoneId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await repository.RackExistsByCodeAsync(
                zoneId,
                code,
                excludeId,
                cancellationToken);

            if (exists)
            {
                throw new WarehouseDomainValidationException(
                    $"Rack code '{code}' already exists in this zone.");
            }
        }

        public static void ValidateRackForActivation(
            RackModel rack)
        {
            if (rack.IsDeleted == true)
            {
                throw new WarehouseDomainValidationException(
                    "Cannot activate a deleted rack.");
            }
        }


        // =============================================================
        // BIN RULES
        // =============================================================

        public static void ValidateBinName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WarehouseDomainValidationException(
                    "Bin name is required.");
            }

            if (name.Length > 100)
            {
                throw new WarehouseDomainValidationException(
                    "Bin name cannot exceed 100 characters.");
            }
        }

        public static void ValidateBinCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new WarehouseDomainValidationException(
                    "Bin code is required.");
            }

            if (code.Length > 20)
            {
                throw new WarehouseDomainValidationException(
                    "Bin code cannot exceed 20 characters.");
            }

            if (!Regex.IsMatch(code, @"^[A-Z0-9\-_]+$"))
            {
                throw new WarehouseDomainValidationException(
                    "Bin code must contain only uppercase letters, numbers, hyphens, and underscores.");
            }
        }

        public static async Task ValidateBinCodeUniquenessAsync(
            IWarehouseRepository repository,
            int rackId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var exists = await repository.BinExistsByCodeAsync(
                rackId,
                code,
                excludeId,
                cancellationToken);

            if (exists)
            {
                throw new WarehouseDomainValidationException(
                    $"Bin code '{code}' already exists in this rack.");
            }
        }

        public static void ValidateBinForActivation(
            BinModel bin)
        {
            if (bin.IsDeleted == true)
            {
                throw new WarehouseDomainValidationException(
                    "Cannot activate a deleted bin.");
            }
        }

        public static void ValidateBinForDeletion(
            BinModel bin)
        {
            if (bin.IsOccupied == true)
            {
                throw new BinAlreadyOccupiedException(
                    bin.BinId);
            }
        }
    }
}
