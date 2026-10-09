using AutoMapper;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;
using System.Linq;

namespace ERPWMS.Application.Mappings
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // =====================================================
            // Employee -> Employee Summary
            // =====================================================
            CreateMap<Employee, EmployeeSummaryResponse>()
                .ForMember(
                    d => d.FullName,
                    o => o.MapFrom(s =>
                        (s.FirstName + " " + s.LastName).Trim())
                )
                .ForMember(
                    d => d.DepartmentName,
                    o => o.MapFrom(s =>
                        s.Department != null
                            ? s.Department.DepartmentName
                            : string.Empty)
                )
                .ForMember(
                    d => d.DesignationName,
                    o => o.MapFrom(s =>
                        s.Designation != null
                            ? s.Designation.DesignationName
                            : string.Empty)
                )
                .ForMember(
                    d => d.BranchName,
                    o => o.MapFrom(s =>
                        s.Branch != null
                            ? s.Branch.BranchName
                            : string.Empty)
                )
                .ForMember(
                    d => d.ShiftName,
                    o => o.MapFrom(s =>
                        s.Shift != null
                            ? s.Shift.ShiftName
                            : string.Empty)
                )
                .ForMember(
                    d => d.AttendancePercentage,
                    o => o.MapFrom(s => 0m)
                );


            // =====================================================
            // Employee -> EmployeeDetails
            // =====================================================
            CreateMap<Employee, EmployeeDetails>()

                // =================================================
                // Personal Information
                // =================================================
                .ForMember(
                    d => d.FullName,
                    o => o.Ignore()
                )

                .ForMember(
                    d => d.Cnic,
                    o => o.MapFrom(s => s.Cnic)
                )

                .ForMember(
                    d => d.Dob,
                    o => o.MapFrom(s => s.DOB)
                )


                // =================================================
                // Employment IDs
                // =================================================
                .ForMember(
                    d => d.DepartmentId,
                    o => o.MapFrom(s => s.DepartmentId)
                )

                .ForMember(
                    d => d.DesignationId,
                    o => o.MapFrom(s => s.DesignationId)
                )

                .ForMember(
                    d => d.BranchId,
                    o => o.MapFrom(s => s.BranchId)
                )

                .ForMember(
                    d => d.WarehouseId,
                    o => o.MapFrom(s => s.WarehouseId)
                )

                .ForMember(
                    d => d.ShiftId,
                    o => o.MapFrom(s => s.ShiftId)
                )


                // =================================================
                // Employment Display Names
                // =================================================
                .ForMember(
                    d => d.DepartmentName,
                    o => o.MapFrom(s =>
                        s.Department != null
                            ? s.Department.DepartmentName
                            : string.Empty)
                )

                .ForMember(
                    d => d.DesignationName,
                    o => o.MapFrom(s =>
                        s.Designation != null
                            ? s.Designation.DesignationName
                            : string.Empty)
                )

                .ForMember(
                    d => d.BranchName,
                    o => o.MapFrom(s =>
                        s.Branch != null
                            ? s.Branch.BranchName
                            : string.Empty)
                )

                .ForMember(
                    d => d.WarehouseName,
                    o => o.MapFrom(s =>
                        s.Warehouse != null
                            ? s.Warehouse.WarehouseName
                            : string.Empty)
                )

                .ForMember(
                    d => d.ShiftName,
                    o => o.MapFrom(s =>
                        s.Shift != null
                            ? s.Shift.ShiftName
                            : string.Empty)
                )


                // =================================================
                // Status
                // =================================================
                .ForMember(
                    d => d.IsActive,
                    o => o.MapFrom(s => s.IsActive)
                )

                .ForMember(
                    d => d.IsDeleted,
                    o => o.MapFrom(s => s.IsDeleted)
                )


                // =================================================
                // Financial
                // =================================================
                .ForMember(
                    d => d.BaseSalary,
                    o => o.MapFrom(s => s.BaseSalary)
                )

                .ForMember(
                    d => d.CurrentNetSalary,
                    o => o.Ignore()
                )


                // =================================================
                // Attendance
                // =================================================
                .ForMember(
                    d => d.AttendancePercentage,
                    o => o.MapFrom(s => 0m)
                )

                .ForMember(
                    d => d.Attendances,
                    o => o.Ignore()
                )


                // =================================================
                // Payroll
                // =================================================
                .ForMember(
                    d => d.Payrolls,
                    o => o.MapFrom(s =>
                        s.EmployeePayrolls
                            .OrderByDescending(p => p.PayrollId))
                )

                .ForMember(
                    d => d.LatestPayroll,
                    o => o.MapFrom(s =>
                        s.EmployeePayrolls
                            .OrderByDescending(p => p.PayrollId)
                            .FirstOrDefault())
                )


                // =================================================
                // Leave History
                // =================================================
                .ForMember(
                    d => d.Leaves,
                    o => o.MapFrom(s =>
                        s.EmployeeLeaves
                            .OrderByDescending(l => l.LeaveId))
                )


                // =================================================
                // Salary History
                // =================================================
                .ForMember(
                    d => d.Salaries,
                    o => o.MapFrom(s =>
                        s.EmployeeSalaries
                            .OrderByDescending(sal => sal.SalaryId))
                )


                // =================================================
                // Bank Accounts
                // =================================================
                .ForMember(
                    d => d.BankAccounts,
                    o => o.Ignore()
                )

                .ForMember(
                    d => d.PrimaryBankAccount,
                    o => o.Ignore()
                );


            // =====================================================
            // EmployeeAttendance -> EmployeeAttendanceDto
            // =====================================================
            CreateMap<EmployeeAttendance, EmployeeAttendanceDto>()
                .ForMember(
                    d => d.ShiftName,
                    o => o.MapFrom(s =>
                        s.Shift != null
                            ? s.Shift.ShiftName
                            : string.Empty)
                );


            // =====================================================
            // EmployeePayroll -> EmployeePayrollDto
            // =====================================================
            CreateMap<EmployeePayroll, EmployeePayrollDto>();


            // =====================================================
            // EmployeeLeave -> EmployeeLeaveDto
            // =====================================================
            CreateMap<EmployeeLeave, EmployeeLeaveDto>()
                .ForMember(
                    d => d.ApprovedDate,
                    o => o.MapFrom(s => s.ApprovedDate)
                );


            // =====================================================
            // EmployeeSalary -> EmployeeSalaryDto
            // =====================================================
            CreateMap<EmployeeSalary, EmployeeSalaryDto>();
        }
    }
}