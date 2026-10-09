using System;
using ERPWMS.Domain.Rules;

namespace ERPWMS.Domain.ValueObjects
{
    public class DesignationContactInfo : IEquatable<DesignationContactInfo>
    {
        public string? Description { get; private set; }
        public int? RankLevel { get; private set; }

        private DesignationContactInfo() { }

        public DesignationContactInfo(string? description, int? rankLevel)
        {
            DesignationBusinessRules.ValidateRankLevel(rankLevel);
            Description = description;
            RankLevel = rankLevel;
        }

        public DesignationContactInfo UpdateDescription(string? description)
        {
            return new DesignationContactInfo(description, RankLevel);
        }

        public DesignationContactInfo UpdateRankLevel(int? rankLevel)
        {
            DesignationBusinessRules.ValidateRankLevel(rankLevel);
            return new DesignationContactInfo(Description, rankLevel);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DesignationContactInfo);
        }

        public bool Equals(DesignationContactInfo? other)
        {
            if (other is null) return false;

            return string.Equals(Description, other.Description, StringComparison.Ordinal) &&
                   RankLevel == other.RankLevel;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Description, RankLevel);
        }

        public static bool operator ==(DesignationContactInfo? left, DesignationContactInfo? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(DesignationContactInfo? left, DesignationContactInfo? right)
        {
            return !(left == right);
        }
    }
}

