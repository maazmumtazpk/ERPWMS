using System;
using ERPWMS.Database.Models;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;


namespace ERPWMS.Application.Commands.Supplier.ChangeSupplierProfile
{
    public class ChangeSupplierProfileCommandHandler : IRequestHandler<ChangeSupplierProfileCommand, SupplierDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public ChangeSupplierProfileCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(ChangeSupplierProfileCommand request, CancellationToken cancellationToken)
        {
            // Find the supplier
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.IsDeleted != true, cancellationToken);

            if (supplier == null)
            {
                throw new SupplierNotFoundException(request.SupplierId);
            }

            // Check if supplier is active
            if (supplier.IsActive != true)
            {
                throw new SupplierInactiveException(request.SupplierId);
            }

            // Check if supplier is deleted
            if (supplier.IsDeleted == true)
            {
                throw new SupplierDeletedException(request.SupplierId);
            }

            // Validate: Check if supplier code already exists (excluding current supplier)
            if (!string.IsNullOrWhiteSpace(request.Request.SupplierCode))
            {
                var existingSupplier = await _context.Suppliers
                    .AnyAsync(s => s.SupplierCode == request.Request.SupplierCode
                        && s.SupplierId != request.SupplierId
                        && s.IsDeleted != true, cancellationToken);

                if (existingSupplier)
                {
                    throw new SupplierCodeAlreadyExistsException(request.Request.SupplierCode);
                }
            }

            // Update supplier properties
            supplier.SupplierName = request.Request.SupplierName;
            supplier.SupplierCode = request.Request.SupplierCode;
            supplier.ContactPerson = request.Request.ContactPerson;
            supplier.Phone = request.Request.Phone;
            supplier.Mobile = request.Request.Mobile;
            supplier.Email = request.Request.Email;
            supplier.Website = request.Request.Website;
            supplier.Address = request.Request.Address;
            supplier.City = request.Request.City;
            supplier.State = request.Request.State;
            supplier.Country = request.Request.Country;
            supplier.PostalCode = request.Request.PostalCode;
            supplier.TaxId = request.Request.TaxId;
            supplier.NTN = request.Request.NTN;
            supplier.BankName = request.Request.BankName;
            supplier.BankAccountNo = request.Request.BankAccountNo;
            supplier.BankBranch = request.Request.BankBranch;
            supplier.CreditLimit = request.Request.CreditLimit;
            supplier.PaymentTerms = request.Request.PaymentTerms;
            supplier.UpdatedBy = request.CurrentUserId.ToString();
            supplier.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(supplier).Reference(s => s.CreatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(supplier).Reference(s => s.UpdatedByNavigation).LoadAsync(cancellationToken);

            var result = _mapper.Map<SupplierDto>(supplier);
            return result;
        }
    }
}
