using MediatR;
using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Employee
{
    // ============================================================
    // GET EMPLOYEE DETAIL
    // ============================================================

    public class GetEmployeeDetailQuery
        : IRequest<EmployeeDetailResponse>
    {
        public int EmployeeId { get; set; }
    }

    public class SearchEmployeesQuery : IRequest<PagedResult<EmployeeSummaryResponse>>
    {
        public string? SearchTerm { get; set; }

        public int? DepartmentId { get; set; }

        public int? DesignationId { get; set; }

        public int? BranchId { get; set; }

        public int? WarehouseId { get; set; }

        public bool? IsActive { get; set; }

        public string? SortBy { get; set; }

        public bool? SortDescending { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    // ============================================================
    // GET ATTENDANCE REPORT
    // ============================================================

    public class GetAttendanceReportQuery
        : IRequest<PagedResult<EmployeeAttendanceDto>>
    {
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public int? BranchId { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    // ============================================================
    // GET PAYROLL REPORT
    // ============================================================

    public class GetPayrollReportQuery
        : IRequest<PagedResult<EmployeePayrollDto>>
    {
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }

        public DateOnly? PeriodStart { get; set; }
        public DateOnly? PeriodEnd { get; set; }

        public string? Status { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    // ============================================================
    // EMPLOYEE DASHBOARD
    // ============================================================

    public class EmployeeDashboardQuery
        : IRequest<EmployeeDashboardSummaryDto>
    {
    }


    // ============================================================
    // PAGED RESULT
    // ============================================================

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling(
                    (double)TotalCount / PageSize)
                : 0;

        public bool HasPrevious =>
            Page > 1;

        public bool HasNext =>
            Page < TotalPages;
    }


    // ============================================================
    // EMPLOYEE SUMMARY RESPONSE
    // ============================================================

    public class EmployeeSummaryResponse
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }
            = string.Empty;

        public string FullName { get; set; }
            = string.Empty;


        public string DepartmentName { get; set; }
            = string.Empty;

        public string DesignationName { get; set; }
            = string.Empty;

        public string BranchName { get; set; }
            = string.Empty;

        public string ShiftName { get; set; }
            = string.Empty;


        public DateOnly HireDate { get; set; }

        public DateOnly? TerminationDate { get; set; }


        public bool IsActive { get; set; }


        public decimal BaseSalary { get; set; }


        public decimal AttendancePercentage { get; set; }
    }


    // ============================================================
    // EMPLOYEE DETAIL RESPONSE
    // ============================================================

    public class EmployeeDetailResponse
        : EmployeeSummaryResponse
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FatherName { get; set; }

        public string? CNIC { get; set; }

        public DateOnly? DOB { get; set; }

        public string? Gender { get; set; }


        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Email { get; set; }


        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int BranchId { get; set; }

        public int? WarehouseId { get; set; }

        public int? ShiftId { get; set; }


        public string Status { get; set; }
            = string.Empty;


        public List<EmployeeAttendanceDto> Attendances
        { get; set; } = new();

        public List<EmployeePayrollDto> Payrolls
        { get; set; } = new();

        public List<EmployeeLeaveDto> Leaves
        { get; set; } = new();

        public List<EmployeeSalaryDto> Salaries
        { get; set; } = new();
    }


    // ============================================================
    // EMPLOYEE ATTENDANCE DTO
    // ============================================================

    public class EmployeeAttendanceDto
    {
        public int AttendanceId { get; set; }

        public DateOnly AttendanceDate { get; set; }


        public TimeOnly? CheckInTime { get; set; }

        public TimeOnly? CheckOutTime { get; set; }


        public string Status { get; set; }
            = string.Empty;


        public decimal? OvertimeHours { get; set; }


        public string ShiftName { get; set; }
            = string.Empty;
    }


    // ============================================================
    // EMPLOYEE PAYROLL DTO
    // ============================================================

    public class EmployeePayrollDto
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }


        public DateOnly MonthYear { get; set; }


        public decimal GrossPay { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetPay { get; set; }


        public string? Status { get; set; }


        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }


        public DateTime? ApprovedDate { get; set; }

        public DateTime? PaymentDate { get; set; }
    }


    // ============================================================
    // EMPLOYEE LEAVE DTO
    // ============================================================

    public class EmployeeLeaveDto
    {
        public int LeaveId { get; set; }


        public string LeaveType { get; set; }
            = string.Empty;


        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }


        public string? ApprovedByName { get; set; }


        public int TotalDays { get; set; }


        public string Reason { get; set; }
            = string.Empty;


        public string Status { get; set; }
            = string.Empty;


        public string? RejectionReason { get; set; }


        public DateTime? ApprovedDate { get; set; }
    }


    // ============================================================
    // EMPLOYEE SALARY DTO
    // ============================================================

    public class EmployeeSalaryDto
    {
        public int SalaryId { get; set; }


        public DateOnly EffectiveDate { get; set; }


        public decimal BasicSalary { get; set; }


        public decimal? HouseRent { get; set; }

        public decimal? UtilityAllowance { get; set; }

        public decimal? MedicalAllowance { get; set; }

        public decimal? OtherAllowances { get; set; }


        public decimal? TotalAllowances { get; set; }


        public decimal? IncomeTax { get; set; }

        public decimal? ProfessionalTax { get; set; }

        public decimal? OtherDeductions { get; set; }


        public decimal? TotalDeductions { get; set; }


        public decimal? NetSalary { get; set; }


        public bool IsActive { get; set; }
    }


    // ============================================================
    // EMPLOYEE DASHBOARD SUMMARY
    // ============================================================

    
    // =====================================================
    // EMPLOYEE DASHBOARD SUMMARY DTO
    // =====================================================
    public class EmployeeDashboardSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int NewThisMonth { get; set; }
        public int OnLeaveToday { get; set; }
        public int PresentToday { get; set; }
        public decimal AverageSalary { get; set; }
        public int DepartmentsCount { get; set; }
        public int DesignationsCount { get; set; }
        public int BranchesCount { get; set; }
        public decimal TotalPayroll { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
    }}