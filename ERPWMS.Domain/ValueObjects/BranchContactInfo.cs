using System;
using ERPWMS.Domain.Rules;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.ValueObjects
{
    /// <summary>
    /// Value object for branch contact information
    /// </summary>
    public class BranchContactInfo : IEquatable<BranchContactInfo>
    {
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }

        private BranchContactInfo() { }

        public BranchContactInfo(string? phone, string? email, string? address)
        {
            BranchBusinessRules.ValidatePhone(phone);
            BranchBusinessRules.ValidateEmail(email);
            BranchBusinessRules.ValidateAddress(address);

            Phone = phone;
            Email = email;
            Address = address;
        }

        public BranchContactInfo UpdatePhone(string? phone)
        {
            return new BranchContactInfo(phone, Email, Address);
        }

        public BranchContactInfo UpdateEmail(string? email)
        {
            return new BranchContactInfo(Phone, email, Address);
        }

        public BranchContactInfo UpdateAddress(string? address)
        {
            return new BranchContactInfo(Phone, Email, address);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BranchContactInfo);
        }

        public bool Equals(BranchContactInfo? other)
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

        public static bool operator ==(BranchContactInfo? left, BranchContactInfo? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BranchContactInfo? left, BranchContactInfo? right)
        {
            return !(left == right);
        }
    }
}