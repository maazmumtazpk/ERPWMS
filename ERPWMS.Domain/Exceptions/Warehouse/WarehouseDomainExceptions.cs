using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.Warehouse
{
    public abstract class WarehouseDomainException : Exception
    {
        public string? ErrorCode { get; }
        protected WarehouseDomainException(string message) : base(message) { }
        protected WarehouseDomainException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class WarehouseNotFoundException : WarehouseDomainException
    {
        public int WarehouseId { get; }
        public WarehouseNotFoundException(int id) : base($"Warehouse with ID {id} not found.", "WH-001")
        {
            WarehouseId = id;
        }
    }

    public class WarehouseCodeAlreadyExistsException : WarehouseDomainException
    {
        public string Code { get; }
        public WarehouseCodeAlreadyExistsException(string code) : base($"Warehouse code '{code}' already exists.", "WH-002")
        {
            Code = code;
        }
    }

    public class WarehouseHasActiveZonesException : WarehouseDomainException
    {
        public int WarehouseId { get; }
        public WarehouseHasActiveZonesException(int id) : base($"Warehouse {id} has active zones and cannot be suspended.", "WH-003")
        {
            WarehouseId = id;
        }
    }

    public class ZoneNotFoundException : WarehouseDomainException
    {
        public int ZoneId { get; }
        public ZoneNotFoundException(int id) : base($"Zone with ID {id} not found.", "ZN-001")
        {
            ZoneId = id;
        }
    }

    public class ZoneAlreadyExistsException : WarehouseDomainException
    {
        public string ZoneName { get; }
        public ZoneAlreadyExistsException(string name) : base($"Zone '{name}' already exists in this warehouse.", "ZN-002")
        {
            ZoneName = name;
        }
    }

    public class RackNotFoundException : WarehouseDomainException
    {
        public int RackId { get; }
        public RackNotFoundException(int id) : base($"Rack with ID {id} not found.", "RK-001")
        {
            RackId = id;
        }
    }

    public class RackAlreadyExistsException : WarehouseDomainException
    {
        public string RackName { get; }
        public RackAlreadyExistsException(string name) : base($"Rack '{name}' already exists in this zone.", "RK-002")
        {
            RackName = name;
        }
    }

    public class BinNotFoundException : WarehouseDomainException
    {
        public int BinId { get; }
        public BinNotFoundException(int id) : base($"Bin with ID {id} not found.", "BN-001")
        {
            BinId = id;
        }
    }

    public class BinAlreadyExistsException : WarehouseDomainException
    {
        public string BinName { get; }
        public BinAlreadyExistsException(string name) : base($"Bin '{name}' already exists in this rack.", "BN-002")
        {
            BinName = name;
        }
    }

    public class BinAlreadyOccupiedException : WarehouseDomainException
    {
        public int BinId { get; }
        public BinAlreadyOccupiedException(int id) : base($"Bin {id} is already occupied.", "BN-003")
        {
            BinId = id;
        }
    }

    public class WarehouseDomainValidationException : WarehouseDomainException
    {
        public WarehouseDomainValidationException(string message) : base(message, "WH-099") { }
    }

    public class UnauthorizedWarehouseAccessException : WarehouseDomainException
    {
        public UnauthorizedWarehouseAccessException(int userId, int warehouseId)
            : base($"User {userId} is not authorized to access warehouse {warehouseId}.", "WH-401")
        {
        }
    }
}
