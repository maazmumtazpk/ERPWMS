using AutoMapper;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Branch entity
    /// </summary>
    public class BranchMappingProfile : Profile
    {
        public BranchMappingProfile()
        {
            // Domain to DTO mappings
            CreateMap<Branch, BranchProfileResponse>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.EmployeeCount, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseCount, opt => opt.Ignore());

            CreateMap<Branch, BranchSummaryResponse>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.EmployeeCount, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseCount, opt => opt.Ignore());

            // Request to Domain mappings
            CreateMap<RegisterBranchRequest, Branch>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore());

            CreateMap<ChangeBranchProfileRequest, Branch>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore());
        }
    }
}