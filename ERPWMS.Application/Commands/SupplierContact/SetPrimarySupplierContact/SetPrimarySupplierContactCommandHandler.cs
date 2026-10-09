using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Supplier.SupplierContact.SetPrimarySupplierContact
{
    public class SetPrimarySupplierContactCommandHandler : IRequestHandler<SetPrimarySupplierContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public SetPrimarySupplierContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SetPrimarySupplierContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.SupplierContacts
                .Include(sc => sc.Supplier)
                .FirstOrDefaultAsync(sc => sc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new SupplierContactNotFoundException(request.ContactId);
            }

            if (contact.Supplier == null || contact.Supplier.IsDeleted == true)
            {
                throw new SupplierNotFoundException(contact.SupplierId);
            }

            if (contact.Supplier.IsActive != true)
            {
                throw new SupplierInactiveException(contact.SupplierId);
            }

            if (contact.IsActive != true)
            {
                throw new SupplierContactInactiveException(request.ContactId);
            }

            // Already primary
            if (contact.IsPrimary == true)
            {
                return true;
            }

            // Clear existing primary
            var existingPrimary = await _context.SupplierContacts
                .FirstOrDefaultAsync(sc => sc.SupplierId == contact.SupplierId
                    && sc.IsPrimary == true
                    && sc.IsActive == true
                    && sc.ContactId != request.ContactId, cancellationToken);

            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
            }

            contact.IsPrimary = true;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
