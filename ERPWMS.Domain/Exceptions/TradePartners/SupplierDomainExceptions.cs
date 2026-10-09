using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.TradePartners
{
    // ============================================================
    // BASE SUPPLIER DOMAIN EXCEPTION
    // ============================================================

    public class SupplierDomainException : TradePartnerDomainException
    {
        public SupplierDomainException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }

        public SupplierDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // SUPPLIER NOT FOUND
    // ============================================================

    public class SupplierNotFoundException : SupplierDomainException
    {
        public int? SupplierId { get; }
        public string? SupplierCode { get; }

        public SupplierNotFoundException(int supplierId)
            : base(
                $"Supplier with ID {supplierId} was not found",
                "SP-001")
        {
            SupplierId = supplierId;
        }

        public SupplierNotFoundException(string supplierCode)
            : base(
                $"Supplier with Code {supplierCode} was not found",
                "SP-001")
        {
            SupplierCode = supplierCode;
        }

        public SupplierNotFoundException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }
    }


    // ============================================================
    // SUPPLIER CODE ALREADY EXISTS
    // ============================================================

    public class SupplierCodeAlreadyExistsException
        : SupplierDomainException
    {
        public string SupplierCode { get; }

        public SupplierCodeAlreadyExistsException(
            string supplierCode)
            : base(
                $"Supplier with code '{supplierCode}' already exists",
                "SP-002")
        {
            SupplierCode = supplierCode;
        }

        public SupplierCodeAlreadyExistsException(
            string supplierCode,
            string message)
            : base(message, "SP-002")
        {
            SupplierCode = supplierCode;
        }
    }


    // ============================================================
    // SUPPLIER INACTIVE
    // ============================================================

    public class SupplierInactiveException : SupplierDomainException
    {
        public int SupplierId { get; }

        public SupplierInactiveException(int supplierId)
            : base(
                $"Supplier with ID {supplierId} is inactive",
                "SP-003")
        {
            SupplierId = supplierId;
        }

        public SupplierInactiveException(
            int supplierId,
            string message)
            : base(message, "SP-003")
        {
            SupplierId = supplierId;
        }
    }


    // ============================================================
    // SUPPLIER DELETED
    // ============================================================

    public class SupplierDeletedException : SupplierDomainException
    {
        public int SupplierId { get; }

        public SupplierDeletedException(int supplierId)
            : base(
                $"Supplier with ID {supplierId} has been deleted and cannot be modified",
                "SP-004")
        {
            SupplierId = supplierId;
        }

        public SupplierDeletedException(
            int supplierId,
            string message)
            : base(message, "SP-004")
        {
            SupplierId = supplierId;
        }
    }


    // ============================================================
    // SUPPLIER CONTACT NOT FOUND
    // ============================================================

    public class SupplierContactNotFoundException
        : SupplierDomainException
    {
        public int ContactId { get; }
        public int? SupplierId { get; }

        public SupplierContactNotFoundException(int contactId)
            : base(
                $"Supplier contact with ID {contactId} was not found",
                "SP-005")
        {
            ContactId = contactId;
        }

        public SupplierContactNotFoundException(
            int contactId,
            int supplierId)
            : base(
                $"Supplier contact with ID {contactId} was not found for supplier {supplierId}",
                "SP-005")
        {
            ContactId = contactId;
            SupplierId = supplierId;
        }
    }


    // ============================================================
    // SUPPLIER CONTACT INACTIVE
    // ============================================================

    public class SupplierContactInactiveException
        : SupplierDomainException
    {
        public int ContactId { get; }

        public SupplierContactInactiveException(int contactId)
            : base(
                $"Supplier contact with ID {contactId} is inactive",
                "SP-006")
        {
            ContactId = contactId;
        }
    }


    // ============================================================
    // SUPPLIER CONTACT PRIMARY EXCEPTION
    // ============================================================

    public class SupplierContactPrimaryException
        : SupplierDomainException
    {
        public int ContactId { get; }
        public int SupplierId { get; }

        public SupplierContactPrimaryException(
            int contactId,
            int supplierId,
            string message)
            : base(message, "SP-007")
        {
            ContactId = contactId;
            SupplierId = supplierId;
        }

        public static SupplierContactPrimaryException ContactCannotBeUpdated(
            int contactId,
            int supplierId)
        {
            return new SupplierContactPrimaryException(
                contactId,
                supplierId,
                $"Cannot update primary contact {contactId} for supplier {supplierId}");
        }

        public static SupplierContactPrimaryException ContactCannotBeDeleted(
            int contactId,
            int supplierId)
        {
            return new SupplierContactPrimaryException(
                contactId,
                supplierId,
                $"Cannot delete primary contact {contactId} for supplier {supplierId}");
        }

        public static SupplierContactPrimaryException SupplierRequiresPrimaryContact(
            int supplierId)
        {
            return new SupplierContactPrimaryException(
                0,
                supplierId,
                $"Supplier {supplierId} must have at least one primary contact");
        }
    }
}