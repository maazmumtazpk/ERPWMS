using AutoMapper;
using ERPWMS.Application.DTOs.Department;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Department entity
    /// </summary>
    public class DepartmentMappingProfile : Profile
    {
        public DepartmentMappingProfile()
        {
            // Domain to DTO mappings
            CreateMap<Department, DepartmentProfileResponse>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.DesignationCount, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCount, opt => opt.Ignore());

            CreateMap<Department, DepartmentSummaryResponse>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.DesignationCount, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCount, opt => opt.Ignore());

            // Request to Domain mappings
            CreateMap<RegisterDepartmentRequest, Department>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Designations, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            CreateMap<ChangeDepartmentProfileRequest, Department>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Designations, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());
        }
    }
}
