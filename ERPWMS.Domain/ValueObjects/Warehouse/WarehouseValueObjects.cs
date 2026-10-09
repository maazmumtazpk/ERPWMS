using System;
using System.Text.RegularExpressions;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.ValueObjects.Warehouse
{
    // ✅ Improved with better validation
    public record WarehouseName
    {
        public string Value { get; }

        public WarehouseName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Warehouse name is required.");

            value = value.Trim();

            if (value.Length > 200)
                throw new ArgumentException("Warehouse name cannot exceed 200 characters.");

            Value = value;
        }

        public override string ToString() => Value;
    }

    public record WarehouseCode
    {
        public string? Value { get; }

        public WarehouseCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            value = value.Trim().ToUpperInvariant();

            if (!Regex.IsMatch(value, @"^[A-Z0-9\-_]+$"))
                throw new ArgumentException("Warehouse code can only contain letters, numbers, hyphens, and underscores.");

            if (value.Length > 20)
                throw new ArgumentException("Warehouse code cannot exceed 20 characters.");

            Value = value;
        }

        public bool HasValue => !string.IsNullOrEmpty(Value);
    }

    public record ZoneName
    {
        public string Value { get; }

        public ZoneName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Zone name is required.");

            value = value.Trim();

            if (value.Length > 100)
                throw new ArgumentException("Zone name cannot exceed 100 characters.");

            Value = value;
        }
    }

    public record ZoneCode
    {
        public string? Value { get; }

        public ZoneCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            value = value.Trim().ToUpperInvariant();

            if (value.Length > 20)
                throw new ArgumentException("Zone code cannot exceed 20 characters.");

            Value = value;
        }
    }

    public record RackName
    {
        public string Value { get; }

        public RackName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Rack name is required.");

            value = value.Trim();

            if (value.Length > 100)
                throw new ArgumentException("Rack name cannot exceed 100 characters.");

            Value = value;
        }
    }

    public record RackCode
    {
        public string? Value { get; }

        public RackCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            value = value.Trim().ToUpperInvariant();

            if (value.Length > 20)
                throw new ArgumentException("Rack code cannot exceed 20 characters.");

            Value = value;
        }
    }

    public record BinName
    {
        public string Value { get; }

        public BinName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Bin name is required.");

            value = value.Trim();

            if (value.Length > 100)
                throw new ArgumentException("Bin name cannot exceed 100 characters.");

            Value = value;
        }
    }

    public record BinCode
    {
        public string? Value { get; }

        public BinCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            value = value.Trim().ToUpperInvariant();

            if (value.Length > 20)
                throw new ArgumentException("Bin code cannot exceed 20 characters.");

            Value = value;
        }
    }

    // ✅ New: Address Value Object
    public record WarehouseAddress
    {
        public string? Street { get; }
        public string? City { get; }
        public string? State { get; }
        public string? Country { get; }
        public string? PostalCode { get; }
        public string FullAddress => string.Join(", ", new[] { Street, City, State, Country, PostalCode }.Where(s => !string.IsNullOrEmpty(s)));

        public WarehouseAddress(string? street, string? city, string? state, string? country, string? postalCode)
        {
            Street = street?.Trim();
            City = city?.Trim();
            State = state?.Trim();
            Country = country?.Trim();
            PostalCode = postalCode?.Trim();
        }
    }

    // ✅ New: Contact Value Object
    public record WarehouseContact
    {
        public string? Phone { get; }
        public string? Email { get; }

        public WarehouseContact(string? phone, string? email)
        {
            Phone = phone?.Trim();
            Email = email?.Trim();

            if (!string.IsNullOrEmpty(Email) && !Email.Contains("@"))
                throw new ArgumentException("Invalid email format.");
        }
    }
}
