using System;
using ERPWMS.Domain.Rules;

namespace ERPWMS.Domain.ValueObjects
{
    public class DepartmentContactInfo : IEquatable<DepartmentContactInfo>
    {
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Address { get; private set; }

        private DepartmentContactInfo() { }

        public DepartmentContactInfo(string? email, string? phone, string? address)
        {
            DepartmentBusinessRules.ValidateEmail(email);
            DepartmentBusinessRules.ValidatePhone(phone);
            DepartmentBusinessRules.ValidateAddress(address);

            Email = email;
            Phone = phone;
            Address = address;
        }

        public DepartmentContactInfo UpdateEmail(string? email)
        {
            return new DepartmentContactInfo(email, Phone, Address);
        }

        public DepartmentContactInfo UpdatePhone(string? phone)
        {
            return new DepartmentContactInfo(Email, phone, Address);
        }

        public DepartmentContactInfo UpdateAddress(string? address)
        {
            return new DepartmentContactInfo(Email, Phone, address);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DepartmentContactInfo);
        }

        public bool Equals(DepartmentContactInfo? other)
        {
            if (other is null) return false;

            return string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Phone, other.Phone, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Email?.ToLowerInvariant(),
                Phone?.ToLowerInvariant(),
                Address?.ToLowerInvariant()
            );
        }

        public static bool operator ==(DepartmentContactInfo? left, DepartmentContactInfo? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(DepartmentContactInfo? left, DepartmentContactInfo? right)
        {
            return !(left == right);
        }
    }
}
