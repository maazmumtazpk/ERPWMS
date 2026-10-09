using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface IWarehouseDomainService
    {
        // Warehouse operations
        Task<Warehouse> CreateWarehouseAsync(
            string warehouseName,
            string warehouseCode,
            int branchId,
            string? address,
            string? phone,
            string? email,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Warehouse> UpdateWarehouseProfileAsync(
            Warehouse warehouse,
            string warehouseName,
            string warehouseCode,
            int branchId,
            string? address,
            string? phone,
            string? email,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Warehouse> ActivateWarehouseAsync(
            Warehouse warehouse,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Warehouse> DeactivateWarehouseAsync(
            Warehouse warehouse,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Warehouse> SoftDeleteWarehouseAsync(
            Warehouse warehouse,
            string deletedBy,
            CancellationToken cancellationToken = default);

        // Zone operations
        Task<Zone> CreateZoneAsync(
            int warehouseId,
            string zoneName,
            string zoneCode,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Zone> UpdateZoneAsync(
            Zone zone,
            int warehouseId,
            string zoneName,
            string zoneCode,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Zone> ActivateZoneAsync(
            Zone zone,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Zone> DeactivateZoneAsync(
            Zone zone,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Zone> SoftDeleteZoneAsync(
            Zone zone,
            string deletedBy,
            CancellationToken cancellationToken = default);

        // Rack operations
        Task<Rack> CreateRackAsync(
            int zoneId,
            string rackName,
            string rackCode,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Rack> UpdateRackAsync(
            Rack rack,
            int zoneId,
            string rackName,
            string rackCode,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Rack> ActivateRackAsync(
            Rack rack,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Rack> DeactivateRackAsync(
            Rack rack,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Rack> SoftDeleteRackAsync(
            Rack rack,
            string deletedBy,
            CancellationToken cancellationToken = default);

        // Bin operations
        Task<Bin> CreateBinAsync(
            int rackId,
            string binName,
            string binCode,
            bool? isOccupied,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Bin> UpdateBinAsync(
            Bin bin,
            int rackId,
            string binName,
            string binCode,
            bool? isOccupied,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Bin> ActivateBinAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Bin> DeactivateBinAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Bin> SoftDeleteBinAsync(
            Bin bin,
            string deletedBy,
            CancellationToken cancellationToken = default);

        Task<Bin> MarkBinOccupiedAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Bin> MarkBinAvailableAsync(
            Bin bin,
            string updatedBy,
            CancellationToken cancellationToken = default);
    }
}


