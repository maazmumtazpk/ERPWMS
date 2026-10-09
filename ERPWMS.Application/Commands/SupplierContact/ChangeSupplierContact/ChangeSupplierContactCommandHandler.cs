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


namespace ERPWMS.Application.Commands.Supplier.SupplierContact.ChangeSupplierContact
{
    public class ChangeSupplierContactCommandHandler : IRequestHandler<ChangeSupplierContactCommand, SupplierContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public ChangeSupplierContactCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierContactDto> Handle(ChangeSupplierContactCommand request, CancellationToken cancellationToken)
        {
            // Find the contact
            var contact = await _context.SupplierContacts
                .Include(sc => sc.Supplier)
                .FirstOrDefaultAsync(sc => sc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new SupplierContactNotFoundException(request.ContactId);
            }

            // Validate: Supplier must exist and be active
            if (contact.Supplier == null || contact.Supplier.IsDeleted == true)
            {
                throw new SupplierNotFoundException(contact.SupplierId);
            }

            if (contact.Supplier.IsActive != true)
            {
                throw new SupplierInactiveException(contact.SupplierId);
            }

            // Check if contact is active
            if (contact.IsActive != true)
            {
                throw new SupplierContactInactiveException(request.ContactId);
            }

            // If setting as primary, handle primary contact logic
            if (request.IsPrimary == true && contact.IsPrimary != true)
            {
                // Clear primary flag from existing primary contact
                var existingPrimary = await _context.SupplierContacts
                    .FirstOrDefaultAsync(sc => sc.SupplierId == contact.SupplierId
                        && sc.IsPrimary == true
                        && sc.IsActive == true
                        && sc.ContactId != request.ContactId, cancellationToken);

                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                }
            }

            // Update contact
            contact.ContactName = request.ContactName;
            contact.Phone = request.Phone;
            contact.Mobile = request.Mobile;
            contact.Email = request.Email;
            contact.Department = request.Department;
            contact.IsPrimary = request.IsPrimary ?? false;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(contact).Reference(sc => sc.CreatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(contact).Reference(sc => sc.UpdatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(contact).Reference(sc => sc.Supplier).LoadAsync(cancellationToken);

            var result = _mapper.Map<SupplierContactDto>(contact);
            return result;
        }
    }
}
