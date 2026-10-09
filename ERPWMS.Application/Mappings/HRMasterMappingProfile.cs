using AutoMapper;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Application.DTOs.EmployeeBankAccount;
using SalaryDto = ERPWMS.Application.DTOs.EmployeeSalary.EmployeeSalaryDto;
using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Mappings
{
    public class HRMasterMappingProfile : Profile
    {
        public HRMasterMappingProfile()
        {
            // ============================================================
            // Employee Mappings
            // ============================================================

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.DepartmentName
                            : null))

                .ForMember(dest => dest.DesignationName,
                    opt => opt.MapFrom(src =>
                        src.Designation != null
                            ? src.Designation.DesignationName
                            : null))

                .ForMember(dest => dest.BranchName,
                    opt => opt.MapFrom(src =>
                        src.Branch != null
                            ? src.Branch.BranchName
                            : null))

                .ForMember(dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse != null
                            ? src.Warehouse.WarehouseName
                            : null))

                .ForMember(dest => dest.ShiftName,
                    opt => opt.MapFrom(src =>
                        src.Shift != null
                            ? src.Shift.ShiftName
                            : null))

                .ForMember(dest => dest.BankAccounts,
                    opt => opt.MapFrom(src =>
                        src.EmployeeBankAccounts ??
                        new List<EmployeeBankAccount>()))

                .ForMember(dest => dest.Salaries,
                    opt => opt.MapFrom(src =>
                        src.EmployeeSalaries ??
                        new List<EmployeeSalary>()));


            // ============================================================
            // Employee List Mapping
            // ============================================================

            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src =>
                        src.Department != null
                            ? src.Department.DepartmentName
                            : null))

                .ForMember(dest => dest.DesignationName,
                    opt => opt.MapFrom(src =>
                        src.Designation != null
                            ? src.Designation.DesignationName
                            : null))

                .ForMember(dest => dest.BranchName,
                    opt => opt.MapFrom(src =>
                        src.Branch != null
                            ? src.Branch.BranchName
                            : null))

                .ForMember(dest => dest.BankAccountCount,
                    opt => opt.MapFrom(src =>
                        src.EmployeeBankAccounts != null
                            ? src.EmployeeBankAccounts.Count(
                                ba => ba.IsActive == true)
                            : 0));


            // ============================================================
            // Employee Dropdown Mapping
            // ============================================================

            CreateMap<Employee, EmployeeDropdownDto>();


            // ============================================================
            // Employee Bank Account Mappings
            // ============================================================

            CreateMap<EmployeeBankAccount, EmployeeBankAccountDto>()
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src =>
                        src.Employee != null
                            ? $"{src.Employee.FirstName} {src.Employee.LastName}"
                            : null))

                .ForMember(dest => dest.EmployeeCode,
                    opt => opt.MapFrom(src =>
                        src.Employee != null
                            ? src.Employee.EmployeeCode
                            : null));


            // ============================================================
            // Employee Salary Mappings
            // ============================================================

            // SalaryDto alias is used to avoid EmployeeSalaryDto ambiguity
            CreateMap<EmployeeSalary, SalaryDto>()
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src =>
                        src.Employee != null
                            ? $"{src.Employee.FirstName} {src.Employee.LastName}"
                            : null))

                .ForMember(dest => dest.EmployeeCode,
                    opt => opt.MapFrom(src =>
                        src.Employee != null
                            ? src.Employee.EmployeeCode
                            : null));


            // ============================================================
            // Shift Mappings
            // ============================================================

            CreateMap<Shift, ShiftDto>()
                .ForMember(dest => dest.EmployeeCount,
                    opt => opt.MapFrom(src =>
                        src.Employees != null
                            ? src.Employees.Count(
                                e => e.IsDeleted != true)
                            : 0));


            // ============================================================
            // Shift List Mappings
            // ============================================================

            CreateMap<Shift, ShiftListDto>()
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src =>
                        src.StartTime.ToString()))

                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src =>
                        src.EndTime.ToString()))

                .ForMember(dest => dest.EmployeeCount,
                    opt => opt.MapFrom(src =>
                        src.Employees != null
                            ? src.Employees.Count(
                                e => e.IsDeleted != true)
                            : 0));
        }
    }
}