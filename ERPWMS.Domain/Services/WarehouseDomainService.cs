using System;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

using ERPWMS.Domain.Services;

namespace ERPWMS.Domain.Services
{
    public class WarehouseDomainService : IWarehouseDomainService
    {
        private readonly IWarehouseRepository _repository;

        public WarehouseDomainService(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        // ============================================================
        // WAREHOUSE OPERATIONS
        // ============================================================

        public async Task<Warehouse> CreateWarehouseAsync(
            string warehouseName,
            string warehouseCode,
            int branchId,
            string? address,
            string? phone,
            string? email,
            string createdBy,
            CancellationToken cancellationToken = default)
        {
            if (await _repository.ExistsByCodeAsync(warehouseCode, null, cancellationToken))
                throw new InvalidOperationException($"Warehouse with code '{warehouseCode}' already exists.");

            var warehouse = new Warehouse
            {
                WarehouseName = warehouseName,
                WarehouseCode = warehouseCode,
                BranchId = branchId,
                Address = address,
                Phone = phone,
                Email = email,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddWarehouseAsync(warehouse, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return warehouse;
        }

        public async Task<Warehouse> UpdateWarehouseProfileAsync(
            Warehouse warehouse,
            string warehouseName,
            string warehouseCode,
            int branchId,
            string? address,
            string? phone,
            string? email,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            if (await _repository.ExistsByCodeAsync(warehouseCode, warehouse.WarehouseId, cancellationToken))
                throw new InvalidOperationException($"Warehouse with code '{warehouseCode}' already exists.");

            warehouse.WarehouseName = warehouseName;
            warehouse.WarehouseCode = warehouseCode;
            warehouse.BranchId = branchId;
            warehouse.Address = address;
            warehouse.Phone = phone;
            warehouse.Email = email;
            warehouse.UpdatedBy = updatedBy;
            warehouse.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateWarehouseAsync(warehouse, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return warehouse;
        }

        public async Task<Warehouse> ActivateWarehouseAsync(
            Warehouse warehouse,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            warehouse.IsActive = true;
            warehouse.IsDeleted = false;
            warehouse.UpdatedBy = updatedBy;
            warehouse.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateWarehouseAsync(warehouse, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return warehouse;
        }

        public async Task<Warehouse> DeactivateWarehouseAsync(
            Warehouse warehouse,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            warehouse.IsActive = false;
            warehouse.UpdatedBy = updatedBy;
            warehouse.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateWarehouseAsync(warehouse, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return warehouse;
        }

        public async Task<Warehouse> SoftDeleteWarehouseAsync(
            Warehouse warehouse,
            string deletedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            warehouse.IsDeleted = true;
            warehouse.IsActive = false;
            warehouse.DeletedBy = deletedBy;
            warehouse.DeletedDate = DateTime.UtcNow;

            await _repository.UpdateWarehouseAsync(warehouse, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return warehouse;
        }

        // ============================================================
        // ZONE OPERATIONS
        // ============================================================

        public async Task<Zone> CreateZoneAsync(
            int warehouseId,
            string zoneName,
            string zoneCode,
            string createdBy,
            CancellationToken cancellationToken = default)
        {
            if (await _repository.ZoneExistsByCodeAsync(warehouseId, zoneCode, null, cancellationToken))
                throw new InvalidOperationException($"Zone with code '{zoneCode}' already exists in this warehouse.");

            var zone = new Zone
            {
                WarehouseId = warehouseId,
                ZoneName = zoneName,
                ZoneCode = zoneCode,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddZoneAsync(zone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return zone;
        }

        public async Task<Zone> UpdateZoneAsync(
            Zone zone,
            int warehouseId,
            string zoneName,
            string zoneCode,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(zone);

            if (await _repository.ZoneExistsByCodeAsync(warehouseId, zoneCode, zone.ZoneId, cancellationToken))
                throw new InvalidOperationException($"Zone with code '{zoneCode}' already exists in this warehouse.");

            zone.WarehouseId = warehouseId;
            zone.ZoneName = zoneName;
            zone.ZoneCode = zoneCode;
            zone.UpdatedBy = updatedBy;
            zone.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateZoneAsync(zone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return zone;
        }

        public async Task<Zone> ActivateZoneAsync(
            Zone zone,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(zone);

            zone.IsActive = true;
            zone.IsDeleted = false;
            zone.UpdatedBy = updatedBy;
            zone.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateZoneAsync(zone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return zone;
        }

        public async Task<Zone> DeactivateZoneAsync(
            Zone zone,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(zone);

            zone.IsActive = false;
            zone.UpdatedBy = updatedBy;
            zone.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateZoneAsync(zone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return zone;
        }

        public async Task<Zone> SoftDeleteZoneAsync(
            Zone zone,
            string deletedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(zone);

            zone.IsDeleted = true;
            zone.IsActive = false;
            zone.DeletedBy = deletedBy;
            zone.DeletedDate = DateTime.UtcNow;

            await _repository.UpdateZoneAsync(zone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return zone;
        }

        // ============================================================
        // RACK OPERATIONS
        // ============================================================

        public async Task<Rack> CreateRackAsync(
            int zoneId,
            string rackName,
            string rackCode,
            string createdBy,
            CancellationToken cancellationToken = default)
        {
            if (await _repository.RackExistsByCodeAsync(zoneId, rackCode, null, cancellationToken))
                throw new InvalidOperationException($"Rack with code '{rackCode}' already exists in this zone.");

            var rack = new Rack
            {
                ZoneId = zoneId,
                RackName = rackName,
                RackCode = rackCode,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddRackAsync(rack, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return rack;
        }

        public async Task<Rack> UpdateRackAsync(
            Rack rack,
            int zoneId,
            string rackName,
            string rackCode,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rack);

            if (await _repository.RackExistsByCodeAsync(zoneId, rackCode, rack.RackId, cancellationToken))
                throw new InvalidOperationException($"Rack with code '{rackCode}' already exists in this zone.");

            rack.ZoneId = zoneId;
            rack.RackName = rackName;
            rack.RackCode = rackCode;
            rack.UpdatedBy = updatedBy;
            rack.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateRackAsync(rack, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return rack;
        }

        public async Task<Rack> ActivateRackAsync(
            Rack rack,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rack);

            rack.IsActive = true;
            rack.IsDeleted = false;
            rack.UpdatedBy = updatedBy;
            rack.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateRackAsync(rack, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return rack;
        }

        public async Task<Rack> DeactivateRackAsync(
            Rack rack,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rack);

            rack.IsActive = false;
            rack.UpdatedBy = updatedBy;
            rack.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateRackAsync(rack, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return rack;
        }

        public async Task<Rack> SoftDeleteRackAsync(
            Rack rack,
            string deletedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rack);

            rack.IsDeleted = true;
            rack.IsActive = false;
            rack.DeletedBy = deletedBy;
            rack.DeletedDate = DateTime.UtcNow;

            await _repository.UpdateRackAsync(rack, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return rack;
        }

        // ============================================================
        // BIN OPERATIONS
        // ============================================================

        public async Task<Bin> CreateBinAsync(
            int rackId,
            string binName,
            string binCode,
            bool? isOccupied,
            string createdBy,
            CancellationToken cancellationToken = default)
        {
            if (await _repository.BinExistsByCodeAsync(rackId, binCode, null, cancellationToken))
                throw new InvalidOperationException($"Bin with code '{binCode}' already exists in this rack.");

            var bin = new Bin
            {
                RackId = rackId,
                BinName = binName,
                BinCode = binCode,
                IsOccupied = isOccupied ?? false,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> UpdateBinAsync(
            Bin bin,
            int rackId,
            string binName,
            string binCode,
            bool? isOccupied,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            if (await _repository.BinExistsByCodeAsync(rackId, binCode, bin.BinId, cancellationToken))
                throw new InvalidOperationException($"Bin with code '{binCode}' already exists in this rack.");

            bin.RackId = rackId;
            bin.BinName = binName;
            bin.BinCode = binCode;
            bin.IsOccupied = isOccupied ?? bin.IsOccupied;
            bin.UpdatedBy = updatedBy;
            bin.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> ActivateBinAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            bin.IsActive = true;
            bin.IsDeleted = false;
            bin.UpdatedBy = updatedBy;
            bin.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> DeactivateBinAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            bin.IsActive = false;
            bin.UpdatedBy = updatedBy;
            bin.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> SoftDeleteBinAsync(
            Bin bin,
            string deletedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            bin.IsDeleted = true;
            bin.IsActive = false;
            bin.DeletedBy = deletedBy;
            bin.DeletedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> MarkBinOccupiedAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            bin.IsOccupied = true;
            bin.UpdatedBy = updatedBy;
            bin.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }

        public async Task<Bin> MarkBinAvailableAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bin);

            bin.IsOccupied = false;
            bin.UpdatedBy = updatedBy;
            bin.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateBinAsync(bin, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return bin;
        }
    }
}
