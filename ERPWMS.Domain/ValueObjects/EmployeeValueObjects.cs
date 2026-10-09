using System;
using System.Text.RegularExpressions;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.ValueObjects
{
    public record EmployeeName
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => $"{FirstName} {LastName}";

        public EmployeeName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 100)
                throw new ArgumentException("First name cannot exceed 100 characters.");
            if (lastName.Length > 100)
                throw new ArgumentException("Last name cannot exceed 100 characters.");

            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString() => FullName;
    }

    public record EmployeeCode
    {
        public string? Value { get; }
        public EmployeeCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }
            value = value.Trim().ToUpperInvariant();
            if (!Regex.IsMatch(value, @"^[A-Z0-9\-_]+$"))
                throw new ArgumentException("Employee code can only contain letters, numbers, hyphens, and underscores.");
            if (value.Length > 50)
                throw new ArgumentException("Employee code cannot exceed 50 characters.");
            Value = value;
        }
        public bool HasValue => !string.IsNullOrEmpty(Value);
    }

    public record CNIC
    {
        public string? Value { get; }
        public CNIC(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }
            value = value.Trim();
            if (!Regex.IsMatch(value, @"^[0-9]{5}-[0-9]{7}-[0-9]{1}$"))
                throw new ArgumentException("Invalid CNIC format. Expected: 12345-1234567-1");
            Value = value;
        }
        public bool HasValue => !string.IsNullOrEmpty(Value);
    }

    public record EmployeeEmail
    {
        public string? Value { get; }
        public EmployeeEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }
            value = value.Trim();
            if (!value.Contains("@") || !value.Contains("."))
                throw new ArgumentException("Invalid email format.");
            if (value.Length > 100)
                throw new ArgumentException("Email cannot exceed 100 characters.");
            Value = value.ToLowerInvariant();
        }
        public bool HasValue => !string.IsNullOrEmpty(Value);
    }

    public record EmployeePhone
    {
        public string? Value { get; }
        public EmployeePhone(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }
            value = value.Trim();
            if (!Regex.IsMatch(value, @"^[0-9+\-\(\)\s]{7,20}$"))
                throw new ArgumentException("Invalid phone number format.");
            Value = value;
        }
        public bool HasValue => !string.IsNullOrEmpty(Value);
    }

    public record EmployeeAddress
    {
        public string? Street { get; }
        public string? City { get; }
        public string? State { get; }
        public string? Country { get; }
        public string? PostalCode { get; }

        public string FullAddress => string.Join(", ", new[] { Street, City, State, Country, PostalCode }
            .Where(s => !string.IsNullOrEmpty(s)));

        public EmployeeAddress(string? street, string? city, string? state, string? country, string? postalCode)
        {
            Street = street?.Trim();
            City = city?.Trim();
            State = state?.Trim();
            Country = country?.Trim();
            PostalCode = postalCode?.Trim();
        }
    }

    public record EmployeeSalary
    {
        public decimal BaseSalary { get; }
        public EmployeeSalary(decimal baseSalary)
        {
            if (baseSalary < 0)
                throw new ArgumentException("Salary cannot be negative.");
            BaseSalary = baseSalary;
        }
    }

    public record EmployeeHireDate
    {
        public DateOnly Value { get; }
        public EmployeeHireDate(DateOnly hireDate)
        {
            if (hireDate > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Hire date cannot be in the future.");
            Value = hireDate;
        }
    }
}
