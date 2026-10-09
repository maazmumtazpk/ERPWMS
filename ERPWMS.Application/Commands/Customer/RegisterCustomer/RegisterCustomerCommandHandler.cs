using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, CustomerDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public RegisterCustomerCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            // Validate: Check if customer code already exists
            if (!string.IsNullOrWhiteSpace(request.Request.CustomerCode))
            {
                var existingCustomer = await _context.Customers
                    .AnyAsync(c => c.CustomerCode == request.Request.CustomerCode
                        && c.IsDeleted != true, cancellationToken);

                if (existingCustomer)
                {
                    throw new CustomerCodeAlreadyExistsException(request.Request.CustomerCode);
                }
            }

            // Create new customer entity
            var customer = new ERPWMS.Database.Models.Customer
            {
                CustomerName = request.Request.CustomerName,
                CustomerCode = request.Request.CustomerCode,
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
                CreditLimit = request.Request.CreditLimit,
                CreditDays = request.Request.CreditDays,
                PaymentTerms = request.Request.PaymentTerms,
                IsActive = true,
                IsDeleted = false,
                // ✅ FIX: int → string
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ FIX: CreatedByNavigation line HATA di (NotMapped hai — load nahi ho sakta)
            // await _context.Entry(customer).Reference(c => c.CreatedByNavigation).LoadAsync(cancellationToken);

            var result = _mapper.Map<CustomerDto>(customer);
            return result;
        }
    }
}