using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Supplier.SupplierContact.DeactivateSupplierContact
{
    public class DeactivateSupplierContactCommandHandler : IRequestHandler<DeactivateSupplierContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeactivateSupplierContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeactivateSupplierContactCommand request, CancellationToken cancellationToken)
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

            // Already inactive
            if (contact.IsActive != true)
            {
                return true;
            }

            // Check if this is the only primary contact
            if (contact.IsPrimary == true)
            {
                var otherActiveContacts = await _context.SupplierContacts
                    .CountAsync(sc => sc.SupplierId == contact.SupplierId
                        && sc.IsActive == true
                        && sc.ContactId != request.ContactId, cancellationToken);

                if (otherActiveContacts == 0)
                {
                    throw SupplierContactPrimaryException.SupplierRequiresPrimaryContact(contact.SupplierId);
                }
            }

            contact.IsActive = false;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
