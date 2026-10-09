using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.TradePartners
{
    // ============================================================
    // BASE CUSTOMER DOMAIN EXCEPTION
    // ============================================================

    public class CustomerDomainException : TradePartnerDomainException
    {
        public CustomerDomainException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }

        public CustomerDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // CUSTOMER NOT FOUND
    // ============================================================

    public class CustomerNotFoundException : CustomerDomainException
    {
        public int? CustomerId { get; }
        public string? CustomerCode { get; }

        public CustomerNotFoundException(int customerId)
            : base(
                $"Customer with ID {customerId} was not found",
                "CM-001")
        {
            CustomerId = customerId;
        }

        public CustomerNotFoundException(string customerCode)
            : base(
                $"Customer with Code {customerCode} was not found",
                "CM-001")
        {
            CustomerCode = customerCode;
        }

        public CustomerNotFoundException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }
    }


    // ============================================================
    // CUSTOMER CODE ALREADY EXISTS
    // ============================================================

    public class CustomerCodeAlreadyExistsException : CustomerDomainException
    {
        public string CustomerCode { get; }

        public CustomerCodeAlreadyExistsException(
            string customerCode)
            : base(
                $"Customer with code '{customerCode}' already exists",
                "CM-002")
        {
            CustomerCode = customerCode;
        }

        public CustomerCodeAlreadyExistsException(
            string customerCode,
            string message)
            : base(message, "CM-002")
        {
            CustomerCode = customerCode;
        }
    }


    // ============================================================
    // CUSTOMER INACTIVE
    // ============================================================

    public class CustomerInactiveException : CustomerDomainException
    {
        public int CustomerId { get; }

        public CustomerInactiveException(int customerId)
            : base(
                $"Customer with ID {customerId} is inactive",
                "CM-003")
        {
            CustomerId = customerId;
        }

        public CustomerInactiveException(
            int customerId,
            string message)
            : base(message, "CM-003")
        {
            CustomerId = customerId;
        }
    }


    // ============================================================
    // CUSTOMER DELETED
    // ============================================================

    public class CustomerDeletedException : CustomerDomainException
    {
        public int CustomerId { get; }

        public CustomerDeletedException(int customerId)
            : base(
                $"Customer with ID {customerId} has been deleted and cannot be modified",
                "CM-004")
        {
            CustomerId = customerId;
        }

        public CustomerDeletedException(
            int customerId,
            string message)
            : base(message, "CM-004")
        {
            CustomerId = customerId;
        }
    }


    // ============================================================
    // CUSTOMER CONTACT NOT FOUND
    // ============================================================

    public class CustomerContactNotFoundException : CustomerDomainException
    {
        public int ContactId { get; }
        public int? CustomerId { get; }

        public CustomerContactNotFoundException(int contactId)
            : base(
                $"Customer contact with ID {contactId} was not found",
                "CM-005")
        {
            ContactId = contactId;
        }

        public CustomerContactNotFoundException(
            int contactId,
            int customerId)
            : base(
                $"Customer contact with ID {contactId} was not found for customer {customerId}",
                "CM-005")
        {
            ContactId = contactId;
            CustomerId = customerId;
        }
    }


    // ============================================================
    // CUSTOMER CONTACT INACTIVE
    // ============================================================

    public class CustomerContactInactiveException : CustomerDomainException
    {
        public int ContactId { get; }

        public CustomerContactInactiveException(int contactId)
            : base(
                $"Customer contact with ID {contactId} is inactive",
                "CM-006")
        {
            ContactId = contactId;
        }
    }


    // ============================================================
    // CUSTOMER CONTACT PRIMARY
    // ============================================================

    public class CustomerContactPrimaryException : CustomerDomainException
    {
        public int ContactId { get; }
        public int CustomerId { get; }

        public CustomerContactPrimaryException(
            int contactId,
            int customerId,
            string message)
            : base(message, "CM-007")
        {
            ContactId = contactId;
            CustomerId = customerId;
        }

        public static CustomerContactPrimaryException ContactCannotBeUpdated(
            int contactId,
            int customerId)
        {
            return new CustomerContactPrimaryException(
                contactId,
                customerId,
                $"Cannot update primary contact {contactId} for customer {customerId}");
        }

        public static CustomerContactPrimaryException ContactCannotBeDeleted(
            int contactId,
            int customerId)
        {
            return new CustomerContactPrimaryException(
                contactId,
                customerId,
                $"Cannot delete primary contact {contactId} for customer {customerId}");
        }

        public static CustomerContactPrimaryException CustomerRequiresPrimaryContact(
            int customerId)
        {
            return new CustomerContactPrimaryException(
                0,
                customerId,
                $"Customer {customerId} must have at least one primary contact");
        }
    }
}