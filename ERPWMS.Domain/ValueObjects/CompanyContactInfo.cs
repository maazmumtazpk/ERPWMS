using System;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Domain.Rules;

namespace ERPWMS.Domain.ValueObjects
{
   
    public class CompanyContactInfo : IEquatable<CompanyContactInfo>
    {
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }

        private CompanyContactInfo() { }

        public CompanyContactInfo(string? phone, string? email, string? address)
        {
            CompanyBusinessRules.ValidatePhone(phone);
            CompanyBusinessRules.ValidateEmail(email);

            Phone = phone;
            Email = email;
            Address = address;
        }

        public CompanyContactInfo UpdatePhone(string? phone)
        {
            return new CompanyContactInfo(phone, Email, Address);
        }

        public CompanyContactInfo UpdateEmail(string? email)
        {
            return new CompanyContactInfo(Phone, email, Address);
        }

        public CompanyContactInfo UpdateAddress(string? address)
        {
            return new CompanyContactInfo(Phone, Email, address);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CompanyContactInfo);
        }

        public bool Equals(CompanyContactInfo? other)
        {
            if (other is null) return false;

            return string.Equals(Phone, other.Phone, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Phone?.ToLowerInvariant(),
                Email?.ToLowerInvariant(),
                Address?.ToLowerInvariant()
            );
        }

        public static bool operator ==(CompanyContactInfo? left, CompanyContactInfo? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(CompanyContactInfo? left, CompanyContactInfo? right)
        {
            return !(left == right);
        }
    }
}