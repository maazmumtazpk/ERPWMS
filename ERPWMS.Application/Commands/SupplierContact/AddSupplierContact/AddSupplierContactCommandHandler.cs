using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Supplier.SupplierContact.AddSupplierContact
{
    public class AddSupplierContactCommandHandler : IRequestHandler<AddSupplierContactCommand, SupplierContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public AddSupplierContactCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierContactDto> Handle(AddSupplierContactCommand request, CancellationToken cancellationToken)
        {
            // Validate: Supplier must exist and be active
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.IsDeleted != true, cancellationToken);

            if (supplier == null)
            {
                // ✅ FIX: exception int leta hai -> .ToString() hata diya
                throw new SupplierNotFoundException(request.SupplierId);
            }

            if (supplier.IsActive != true)
            {
                // ✅ FIX: exception int leta hai -> .ToString() hata diya
                throw new SupplierInactiveException(request.SupplierId);
            }

            // If this contact is primary, ensure no other primary contact exists
            if (request.IsPrimary == true)
            {
                var existingPrimary = await _context.SupplierContacts
                    .AnyAsync(sc => sc.SupplierId == request.SupplierId
                        && sc.IsPrimary == true
                        && sc.IsActive == true, cancellationToken);

                if (existingPrimary)
                {
                    var currentPrimary = await _context.SupplierContacts
                        .FirstOrDefaultAsync(sc => sc.SupplierId == request.SupplierId
                            && sc.IsPrimary == true
                            && sc.IsActive == true, cancellationToken);

                    if (currentPrimary != null)
                    {
                        currentPrimary.IsPrimary = false;
                    }
                }
            }

            // Create new contact
            var contact = new ERPWMS.Database.Models.SupplierContact
            {
                SupplierId = request.SupplierId,
                ContactName = request.ContactName,
                Phone = request.Phone,
                Mobile = request.Mobile,
                Email = request.Email,
                Department = request.Department,
                IsPrimary = request.IsPrimary ?? false,
                IsActive = true,

                // ✅ FIX: int -> string (CreatedBy string type hai)
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            _context.SupplierContacts.Add(contact);
            await _context.SaveChangesAsync(cancellationToken);

            // CreatedByNavigation NotMapped hai -> load karne ki zaroorat nahi
            await _context.Entry(contact).Reference(sc => sc.Supplier).LoadAsync(cancellationToken);

            var result = _mapper.Map<SupplierContactDto>(contact);
            return result;
        }
    }
}