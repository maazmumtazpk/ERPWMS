using ERPWMS.Domain.Interfaces;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // WAREHOUSE OPERATIONS
        // ============================================================

        public async Task<Warehouse?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .Include(w => w.Branch)
                .FirstOrDefaultAsync(
                    w => w.WarehouseId == id,
                    cancellationToken);
        }

        public async Task<Warehouse?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .Include(w => w.Branch)
                .FirstOrDefaultAsync(
                    w => w.WarehouseCode == code,
                    cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Warehouses
                .AsQueryable();

            query = query.Where(w =>
                w.WarehouseCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(w =>
                    w.WarehouseId != excludeId);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesAsync(
            string? searchTerm = null,
            int? branchId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            string? sortBy = null,
            string? sortDirection = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            // ========================================================
            // BASE QUERY
            // ========================================================

            var query = _context.Warehouses
                .AsNoTracking()
                .Include(w => w.Branch)
                .Include(w => w.Zones)
                    .ThenInclude(z => z.Racks)
                        .ThenInclude(r => r.Bins)
                .AsQueryable();

            // ========================================================
            // SEARCH
            // ========================================================

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(w =>
                    w.WarehouseName.Contains(searchTerm) ||
                    (w.WarehouseCode != null &&
                     w.WarehouseCode.Contains(searchTerm)));
            }

            // ========================================================
            // BRANCH FILTER
            // ========================================================

            if (branchId.HasValue)
            {
                query = query.Where(w =>
                    w.BranchId == branchId);
            }

            // ========================================================
            // ACTIVE FILTER
            // ========================================================

            if (isActive.HasValue)
            {
                query = query.Where(w =>
                    w.IsActive == isActive);
            }

            // ========================================================
            // DELETED FILTER
            // ========================================================

            if (isDeleted.HasValue)
            {
                query = query.Where(w =>
                    w.IsDeleted == isDeleted);
            }

            // ========================================================
            // SORTING
            // ========================================================

            bool desc =
                string.Equals(
                    sortDirection,
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.Trim().ToLower())
                {
                    case "name":

                        query = desc
                            ? query.OrderByDescending(
                                w => w.WarehouseName)
                            : query.OrderBy(
                                w => w.WarehouseName);

                        break;

                    case "code":

                        query = desc
                            ? query.OrderByDescending(
                                w => w.WarehouseCode)
                            : query.OrderBy(
                                w => w.WarehouseCode);

                        break;

                    case "branch":

                        query = desc
                            ? query.OrderByDescending(
                                w => w.BranchId)
                            : query.OrderBy(
                                w => w.BranchId);

                        break;

                    case "createddate":

                        query = desc
                            ? query.OrderByDescending(
                                w => w.CreatedDate)
                            : query.OrderBy(
                                w => w.CreatedDate);

                        break;

                    default:

                        query = query.OrderBy(
                            w => w.WarehouseId);

                        break;
                }
            }
            else
            {
                query = query.OrderBy(
                    w => w.WarehouseName);
            }

            // ========================================================
            // PAGINATION
            // ========================================================

            if (pageNumber.HasValue &&
                pageSize.HasValue)
            {
                int page =
                    pageNumber <= 0
                        ? 1
                        : pageNumber.Value;

                int size =
                    pageSize <= 0
                        ? 10
                        : pageSize.Value;

                query = query
                    .Skip((page - 1) * size)
                    .Take(size);
            }

            // ========================================================
            // EXECUTE QUERY
            // ========================================================

            return await query.ToListAsync(
                cancellationToken);
        }

        public async Task<int> GetWarehousesCountAsync(
            string? searchTerm = null,
            int? branchId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Warehouses
                .AsNoTracking()
                .AsQueryable();

            // ========================================================
            // SEARCH
            // ========================================================

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(w =>
                    w.WarehouseName.Contains(searchTerm) ||
                    (w.WarehouseCode != null &&
                     w.WarehouseCode.Contains(searchTerm)));
            }

            // ========================================================
            // BRANCH
            // ========================================================

            if (branchId.HasValue)
            {
                query = query.Where(w =>
                    w.BranchId == branchId);
            }

            // ========================================================
            // ACTIVE
            // ========================================================

            if (isActive.HasValue)
            {
                query = query.Where(w =>
                    w.IsActive == isActive);
            }

            // ========================================================
            // DELETED
            // ========================================================

            if (isDeleted.HasValue)
            {
                query = query.Where(w =>
                    w.IsDeleted == isDeleted);
            }

            return await query.CountAsync(
                cancellationToken);
        }

        public async Task<Warehouse?> GetWarehouseDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Warehouses
                .Include(w => w.Branch)
                .Include(w => w.Zones)
                    .ThenInclude(z => z.Racks)
                        .ThenInclude(r => r.Bins)
                .FirstOrDefaultAsync(
                    w => w.WarehouseId == id,
                    cancellationToken);
        }

        public async Task<bool> HasActiveZonesAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Zones
                .AnyAsync(
                    z =>
                        z.WarehouseId == warehouseId &&
                        z.IsActive == true,
                    cancellationToken);
        }

        // ============================================================
        // ZONE OPERATIONS
        // ============================================================

        public async Task<Zone?> GetZoneByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Zones
                .Include(z => z.Warehouse)
                    .ThenInclude(w => w.Branch)
                .FirstOrDefaultAsync(
                    z => z.ZoneId == id,
                    cancellationToken);
        }

        public async Task<Zone?> GetZoneByCodeAsync(
            int warehouseId,
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.Zones
                .Include(z => z.Warehouse)
                .FirstOrDefaultAsync(
                    z =>
                        z.WarehouseId == warehouseId &&
                        z.ZoneCode == code,
                    cancellationToken);
        }

        public async Task<bool> ZoneExistsByCodeAsync(
            int warehouseId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Zones
                .Where(z =>
                    z.WarehouseId == warehouseId &&
                    z.ZoneCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(z =>
                    z.ZoneId != excludeId);
            }

            return await query.AnyAsync(
                cancellationToken);
        }

        public async Task<IEnumerable<Zone>> GetZonesAsync(
            int? warehouseId = null,
            string? searchTerm = null,
            bool? isActive = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Zones
                .AsNoTracking()
                .Include(z => z.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(z =>
                    z.WarehouseId == warehouseId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(z =>
                    z.ZoneName.Contains(searchTerm) ||
                    (z.ZoneCode != null &&
                     z.ZoneCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(z =>
                    z.IsActive == isActive);
            }

            query = query.OrderBy(
                z => z.ZoneName);

            if (pageNumber.HasValue &&
                pageSize.HasValue)
            {
                int page =
                    pageNumber <= 0
                        ? 1
                        : pageNumber.Value;

                int size =
                    pageSize <= 0
                        ? 10
                        : pageSize.Value;

                query = query
                    .Skip((page - 1) * size)
                    .Take(size);
            }

            return await query.ToListAsync(
                cancellationToken);
        }

        public async Task<int> GetZonesCountAsync(
            int? warehouseId = null,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Zones
                .AsNoTracking()
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(z =>
                    z.WarehouseId == warehouseId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(z =>
                    z.ZoneName.Contains(searchTerm) ||
                    (z.ZoneCode != null &&
                     z.ZoneCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(z =>
                    z.IsActive == isActive);
            }

            return await query.CountAsync(
                cancellationToken);
        }

        // ============================================================
        // RACK OPERATIONS
        // ============================================================

        public async Task<Rack?> GetRackByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Racks
                .Include(r => r.Zone)
                    .ThenInclude(z => z.Warehouse)
                .FirstOrDefaultAsync(
                    r => r.RackId == id,
                    cancellationToken);
        }

        public async Task<Rack?> GetRackByCodeAsync(
            int zoneId,
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.Racks
                .Include(r => r.Zone)
                .FirstOrDefaultAsync(
                    r =>
                        r.ZoneId == zoneId &&
                        r.RackCode == code,
                    cancellationToken);
        }

        public async Task<bool> RackExistsByCodeAsync(
            int zoneId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Racks
                .Where(r =>
                    r.ZoneId == zoneId &&
                    r.RackCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(r =>
                    r.RackId != excludeId);
            }

            return await query.AnyAsync(
                cancellationToken);
        }

        public async Task<IEnumerable<Rack>> GetRacksAsync(
            int? warehouseId = null,
            int? zoneId = null,
            string? searchTerm = null,
            bool? isActive = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Racks
                .AsNoTracking()
                .Include(r => r.Zone)
                    .ThenInclude(z => z.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(r =>
                    r.Zone.WarehouseId ==
                    warehouseId);
            }

            if (zoneId.HasValue)
            {
                query = query.Where(r =>
                    r.ZoneId == zoneId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(r =>
                    r.RackName.Contains(searchTerm) ||
                    (r.RackCode != null &&
                     r.RackCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(r =>
                    r.IsActive == isActive);
            }

            query = query.OrderBy(
                r => r.RackName);

            if (pageNumber.HasValue &&
                pageSize.HasValue)
            {
                int page =
                    pageNumber <= 0
                        ? 1
                        : pageNumber.Value;

                int size =
                    pageSize <= 0
                        ? 10
                        : pageSize.Value;

                query = query
                    .Skip((page - 1) * size)
                    .Take(size);
            }

            return await query.ToListAsync(
                cancellationToken);
        }

        public async Task<int> GetRacksCountAsync(
            int? warehouseId = null,
            int? zoneId = null,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Racks
                .AsNoTracking()
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(r =>
                    r.Zone.WarehouseId ==
                    warehouseId);
            }

            if (zoneId.HasValue)
            {
                query = query.Where(r =>
                    r.ZoneId == zoneId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(r =>
                    r.RackName.Contains(searchTerm) ||
                    (r.RackCode != null &&
                     r.RackCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(r =>
                    r.IsActive == isActive);
            }

            return await query.CountAsync(
                cancellationToken);
        }

        // ============================================================
        // BIN OPERATIONS
        // ============================================================

        public async Task<Bin?> GetBinByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bins
                .Include(b => b.Rack)
                    .ThenInclude(r => r.Zone)
                        .ThenInclude(z => z.Warehouse)
                .FirstOrDefaultAsync(
                    b => b.BinId == id,
                    cancellationToken);
        }

        public async Task<Bin?> GetBinByCodeAsync(
            int rackId,
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.Bins
                .Include(b => b.Rack)
                .FirstOrDefaultAsync(
                    b =>
                        b.RackId == rackId &&
                        b.BinCode == code,
                    cancellationToken);
        }

        public async Task<bool> BinExistsByCodeAsync(
            int rackId,
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Bins
                .Where(b =>
                    b.RackId == rackId &&
                    b.BinCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(b =>
                    b.BinId != excludeId);
            }

            return await query.AnyAsync(
                cancellationToken);
        }

        public async Task<IEnumerable<Bin>> GetBinsAsync(
            int? warehouseId = null,
            int? zoneId = null,
            int? rackId = null,
            string? searchTerm = null,
            bool? isActive = null,
            bool? isOccupied = null,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Bins
                .AsNoTracking()
                .Include(b => b.Rack)
                    .ThenInclude(r => r.Zone)
                        .ThenInclude(z => z.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(b =>
                    b.Rack.Zone.WarehouseId ==
                    warehouseId);
            }

            if (zoneId.HasValue)
            {
                query = query.Where(b =>
                    b.Rack.ZoneId ==
                    zoneId);
            }

            if (rackId.HasValue)
            {
                query = query.Where(b =>
                    b.RackId == rackId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(b =>
                    b.BinName.Contains(searchTerm) ||
                    (b.BinCode != null &&
                     b.BinCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(b =>
                    b.IsActive == isActive);
            }

            if (isOccupied.HasValue)
            {
                query = query.Where(b =>
                    b.IsOccupied == isOccupied);
            }

            query = query.OrderBy(
                b => b.BinName);

            if (pageNumber.HasValue &&
                pageSize.HasValue)
            {
                int page =
                    pageNumber <= 0
                        ? 1
                        : pageNumber.Value;

                int size =
                    pageSize <= 0
                        ? 10
                        : pageSize.Value;

                query = query
                    .Skip((page - 1) * size)
                    .Take(size);
            }

            return await query.ToListAsync(
                cancellationToken);
        }

        public async Task<int> GetBinsCountAsync(
            int? warehouseId = null,
            int? zoneId = null,
            int? rackId = null,
            string? searchTerm = null,
            bool? isActive = null,
            bool? isOccupied = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Bins
                .AsNoTracking()
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(b =>
                    b.Rack.Zone.WarehouseId ==
                    warehouseId);
            }

            if (zoneId.HasValue)
            {
                query = query.Where(b =>
                    b.Rack.ZoneId ==
                    zoneId);
            }

            if (rackId.HasValue)
            {
                query = query.Where(b =>
                    b.RackId == rackId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(b =>
                    b.BinName.Contains(searchTerm) ||
                    (b.BinCode != null &&
                     b.BinCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(b =>
                    b.IsActive == isActive);
            }

            if (isOccupied.HasValue)
            {
                query = query.Where(b =>
                    b.IsOccupied == isOccupied);
            }

            return await query.CountAsync(
                cancellationToken);
        }

        // ============================================================
        // ADD / UPDATE
        // ============================================================

        public async Task<Warehouse> AddWarehouseAsync(
            Warehouse warehouse,
            CancellationToken cancellationToken = default)
        {
            await _context.Warehouses.AddAsync(
                warehouse,
                cancellationToken);

            return warehouse;
        }

        public async Task UpdateWarehouseAsync(
            Warehouse warehouse,
            CancellationToken cancellationToken = default)
        {
            _context.Warehouses.Update(warehouse);

            await Task.CompletedTask;
        }

        public async Task<Zone> AddZoneAsync(
            Zone zone,
            CancellationToken cancellationToken = default)
        {
            await _context.Zones.AddAsync(
                zone,
                cancellationToken);

            return zone;
        }

        public async Task UpdateZoneAsync(
            Zone zone,
            CancellationToken cancellationToken = default)
        {
            _context.Zones.Update(zone);

            await Task.CompletedTask;
        }

        public async Task<Rack> AddRackAsync(
            Rack rack,
            CancellationToken cancellationToken = default)
        {
            await _context.Racks.AddAsync(
                rack,
                cancellationToken);

            return rack;
        }

        public async Task UpdateRackAsync(
            Rack rack,
            CancellationToken cancellationToken = default)
        {
            _context.Racks.Update(rack);

            await Task.CompletedTask;
        }

        public async Task<Bin> AddBinAsync(
            Bin bin,
            CancellationToken cancellationToken = default)
        {
            await _context.Bins.AddAsync(
                bin,
                cancellationToken);

            return bin;
        }

        public async Task UpdateBinAsync(
            Bin bin,
            CancellationToken cancellationToken = default)
        {
            _context.Bins.Update(bin);

            await Task.CompletedTask;
        }

        // ============================================================
        // UNIT OF WORK
        // ============================================================

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
