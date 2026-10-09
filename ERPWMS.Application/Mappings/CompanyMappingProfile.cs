
using AutoMapper;
using ERPWMS.Application.DTOs.Company;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
	/// <summary>
	/// AutoMapper profile for Company model.
	/// </summary>
	public class CompanyMappingProfile : Profile
	{
		public CompanyMappingProfile()
		{
			// ============================================
			// Company Model -> DTO mappings
			// ============================================

			CreateMap<Company, CompanyProfileResponse>()
				.ForMember(
					dest => dest.RegistrationNumber,
					opt => opt.MapFrom(src => src.RegistrationNo))
				.ForMember(
					dest => dest.BranchCount,
					opt => opt.Ignore())
				.ForMember(
					dest => dest.UserCount,
					opt => opt.Ignore());

			CreateMap<Company, CompanySummaryResponse>()
				.ForMember(
					dest => dest.RegistrationNumber,
					opt => opt.MapFrom(src => src.RegistrationNo))
				.ForMember(
					dest => dest.BranchCount,
					opt => opt.Ignore())
				.ForMember(
					dest => dest.UserCount,
					opt => opt.Ignore());


			// ============================================
			// Register Request -> Company Model
			// ============================================

			CreateMap<RegisterCompanyRequest, Company>()
				.ForMember(
					dest => dest.RegistrationNo,
					opt => opt.MapFrom(src => src.RegistrationNumber))
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
					opt => opt.Ignore());


			// ============================================
			// Change Company Profile Request -> Company Model
			// ============================================

			CreateMap<ChangeCompanyProfileRequest, Company>()
				.ForMember(
					dest => dest.RegistrationNo,
					opt => opt.MapFrom(src => src.RegistrationNumber))
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
					opt => opt.Ignore());
		}
	}
}

