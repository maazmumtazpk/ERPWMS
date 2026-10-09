using AutoMapper;
using ERPWMS.Application.DTOs.Designation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
    public class DesignationMappingProfile : Profile
    {
        public DesignationMappingProfile()
        {
            // =====================================================
            // DESIGNATION -> PROFILE RESPONSE
            // Company comes through Department
            // Designation -> Department -> Company
            // =====================================================

            CreateMap<Designation, DesignationProfileResponse>()
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.DepartmentName
                            : string.Empty))
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src =>
                        src.Department != null &&
                        src.Department.Company != null
                            ? src.Department.Company.CompanyName
                            : string.Empty))
                .ForMember(
                    dest => dest.CompanyId,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.CompanyId
                            : 0));


            // =====================================================
            // DESIGNATION -> SUMMARY RESPONSE
            // =====================================================

            CreateMap<Designation, DesignationSummaryResponse>()
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.DepartmentName
                            : string.Empty))
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src =>
                        src.Department != null &&
                        src.Department.Company != null
                            ? src.Department.Company.CompanyName
                            : string.Empty))
                .ForMember(
                    dest => dest.CompanyId,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.CompanyId
                            : 0));


            // =====================================================
            // REGISTER REQUEST -> DESIGNATION
            // =====================================================

            CreateMap<RegisterDesignationRequest, Designation>()
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.MapFrom(src => true))
                .ForMember(
                    dest => dest.IsDeleted,
                    opt => opt.MapFrom(src => false))
                .ForMember(
                    dest => dest.CreatedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.DeletedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.DeletedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Department,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Employees,
                    opt => opt.Ignore());


            // =====================================================
            // CHANGE PROFILE REQUEST -> DESIGNATION
            // =====================================================

            CreateMap<ChangeDesignationProfileRequest, Designation>()
                .ForMember(
                    dest => dest.UpdatedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UpdatedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.IsDeleted,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.CreatedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.DeletedDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.DeletedBy,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Department,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Employees,
                    opt => opt.Ignore());
        }
    }
}