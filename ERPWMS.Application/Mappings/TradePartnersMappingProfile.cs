using AutoMapper;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
    public class TradePartnersMappingProfile : Profile
    {
        public TradePartnersMappingProfile()
        {
            // Supplier mappings
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.Contacts,
                    opt => opt.MapFrom(src => src.SupplierContacts ?? new List<SupplierContact>()));

            CreateMap<Supplier, SupplierListDto>()
                .ForMember(dest => dest.ContactCount,
                    opt => opt.MapFrom(src => src.SupplierContacts != null
                        ? src.SupplierContacts.Count(sc => sc.IsActive == true)
                        : 0));

            CreateMap<SupplierContact, SupplierContactDto>()
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : null));

            // Customer mappings
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Contacts,
                    opt => opt.MapFrom(src => src.CustomerContacts ?? new List<CustomerContact>()));

            CreateMap<Customer, CustomerListDto>()
                .ForMember(dest => dest.ContactCount,
                    opt => opt.MapFrom(src => src.CustomerContacts != null
                        ? src.CustomerContacts.Count(cc => cc.IsActive == true)
                        : 0));

            CreateMap<CustomerContact, CustomerContactDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CustomerName : null));
        }
    }
}
