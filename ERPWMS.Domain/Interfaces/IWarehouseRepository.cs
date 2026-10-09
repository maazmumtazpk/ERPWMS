using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWarehouseRepository
    {
        // Warehouse operations
        Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Warehouse>> GetWarehousesAsync(
            string? searchTerm = null,
            int? branchId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            string? sortBy = null,
            string? sortDirection = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default);
        Task<int> GetWarehousesCountAsync(
            string? searchTerm = null,
            int? branchId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            CancellationToken cancellationToken = default);
        Task<Warehouse?> GetWarehouseDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> HasActiveZonesAsync(int warehouseId, CancellationToken cancellationToken = default);

        // Zone operations
        Task<Zone?> GetZoneByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Zone?> GetZoneByCodeAsync(int warehouseId, string code, CancellationToken cancellationToken = default);
        Task<bool> ZoneExistsByCodeAsync(int warehouseId, string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Zone>> GetZonesAsync(
            int? warehouseId = null,
            string? searchTerm = null,
            bool? isActive = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default);
        Task<int> GetZonesCountAsync(
            int? warehouseId = null,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);

        // Rack operations
        Task<Rack?> GetRackByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Rack?> GetRackByCodeAsync(int zoneId, string code, CancellationToken cancellationToken = default);
        Task<bool> RackExistsByCodeAsync(int zoneId, string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Rack>> GetRacksAsync(
            int? warehouseId = null,
            int? zoneId = null,
            string? searchTerm = null,
            bool? isActive = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default);
        Task<int> GetRacksCountAsync(
            int? warehouseId = null,
            int? zoneId = null,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);

        // Bin operations
        Task<Bin?> GetBinByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Bin?> GetBinByCodeAsync(int rackId, string code, CancellationToken cancellationToken = default);
        Task<bool> BinExistsByCodeAsync(int rackId, string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Bin>> GetBinsAsync(
            int? warehouseId = null,
            int? zoneId = null,
            int? rackId = null,
            string? searchTerm = null,
            bool? isActive = null,
            bool? isOccupied = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default);
        Task<int> GetBinsCountAsync(
            int? warehouseId = null,
            int? zoneId = null,
            int? rackId = null,
            string? searchTerm = null,
            bool? isActive = null,
            bool? isOccupied = null,
            CancellationToken cancellationToken = default);

        // Add/Update/Delete operations
        Task<Warehouse> AddWarehouseAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
        Task UpdateWarehouseAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
        Task<Zone> AddZoneAsync(Zone zone, CancellationToken cancellationToken = default);
        Task UpdateZoneAsync(Zone zone, CancellationToken cancellationToken = default);
        Task<Rack> AddRackAsync(Rack rack, CancellationToken cancellationToken = default);
        Task UpdateRackAsync(Rack rack, CancellationToken cancellationToken = default);
        Task<Bin> AddBinAsync(Bin bin, CancellationToken cancellationToken = default);
        Task UpdateBinAsync(Bin bin, CancellationToken cancellationToken = default);

        // Unit of Work
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

