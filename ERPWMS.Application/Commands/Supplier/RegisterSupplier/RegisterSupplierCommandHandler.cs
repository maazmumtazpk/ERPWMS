using System;
using ERPWMS.Database.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;

namespace ERPWMS.Application.Commands.Supplier.RegisterSupplier
{
    public class RegisterSupplierCommandHandler : IRequestHandler<RegisterSupplierCommand, SupplierDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public RegisterSupplierCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(RegisterSupplierCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Request.SupplierCode))
            {
                var existingSupplier = await _context.Suppliers
                    .AnyAsync(s => s.SupplierCode == request.Request.SupplierCode
                        && s.IsDeleted != true, cancellationToken);

                if (existingSupplier)
                {
                    throw new SupplierCodeAlreadyExistsException(request.Request.SupplierCode);
                }
            }

            var supplier = new ERPWMS.Database.Models.Supplier
            {
                SupplierName = request.Request.SupplierName,
                SupplierCode = request.Request.SupplierCode,
                ContactPerson = request.Request.ContactPerson,
                Phone = request.Request.Phone,
                Mobile = request.Request.Mobile,
                Email = request.Request.Email,
                Website = request.Request.Website,
                Address = request.Request.Address,
                City = request.Request.City,
                State = request.Request.State,
                Country = request.Request.Country,
                PostalCode = request.Request.PostalCode,
                TaxId = request.Request.TaxId,
                NTN = request.Request.NTN,
                BankName = request.Request.BankName,
                BankAccountNo = request.Request.BankAccountNo,
                BankBranch = request.Request.BankBranch,
                CreditLimit = request.Request.CreditLimit,
                PaymentTerms = request.Request.PaymentTerms,
                IsActive = true,
                IsDeleted = false,

                // ✅ FIX: int → string
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ FIX: CreatedByNavigation line HATA di (NotMapped hai)
            // await _context.Entry(supplier).Reference(s => s.CreatedByNavigation).LoadAsync(cancellationToken);

            var result = _mapper.Map<SupplierDto>(supplier);
            return result;
        }
    }
}