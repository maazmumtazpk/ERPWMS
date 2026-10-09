using ERPWMS.Application.DTOs.Reports;
using ERPWMS.Application.Interfaces;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services.Reports
{
    public class ReportService(AppDbContext context) : IReportService
    {
        // =========================================================
        // GET REPORTS LIST
        // =========================================================
        public async Task<List<ReportDefinitionDto>> GetReportsAsync()
        {
            return await Task.FromResult(new List<ReportDefinitionDto>
            {
                new() { ReportCode = "EMP-LIST",         ReportName = "Employee List",     ReportCategory = "HR" },
                new() { ReportCode = "EMP-ATTENDANCE",   ReportName = "Attendance Report", ReportCategory = "HR" },
                new() { ReportCode = "EMP-LEAVE",        ReportName = "Leave Report",      ReportCategory = "HR" },
                new() { ReportCode = "EMP-SALARY",       ReportName = "Salary Report",     ReportCategory = "HR" },
                new() { ReportCode = "EMP-PAYROLL",      ReportName = "Payroll Report",    ReportCategory = "HR" },
                new() { ReportCode = "INV-STOCK-LEDGER", ReportName = "Stock Ledger",      ReportCategory = "Inventory" },
                new() { ReportCode = "SALES-INVOICE",    ReportName = "Invoice Report",    ReportCategory = "Sales" },
                new() { ReportCode = "CUST-LIST",        ReportName = "Customer List",     ReportCategory = "Master Data" },
                new() { ReportCode = "SUPP-LIST",        ReportName = "Supplier List",     ReportCategory = "Master Data" },
                new() { ReportCode = "PROD-LIST",        ReportName = "Product List",      ReportCategory = "Master Data" },
            });
        }

        // =========================================================
        // GENERATE REPORT
        // =========================================================
        public async Task<ReportResult> GenerateReportAsync(string code, ReportParameter param, int companyId, string userName)
        {
            _ = param;
            _ = companyId;
            _ = userName;

            var r = new ReportResult { ReportCode = code, GeneratedAt = DateTime.Now };

            switch (code)
            {
                case "EMP-LIST":
                    r.ReportName = "Employee List";
                    r.Columns = new List<string> { "Code", "Name", "Department", "Designation", "Salary", "Status" };

                    var emps = await context.Employees
                        .Include(e => e.Department)
                        .Include(e => e.Designation)
                        .Where(e => e.IsDeleted != true)
                        .Take(500)
                        .ToListAsync();

                    foreach (var e in emps)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Code",        e.EmployeeCode ?? "-" },
                            { "Name",        (e.FirstName + " " + (e.LastName ?? "")).Trim() },
                            { "Department",  e.Department != null ? e.Department.DepartmentName : "-" },
                            { "Designation", e.Designation != null ? e.Designation.DesignationName : "-" },
                            { "Salary",      $"Rs {e.BaseSalary:N0}" },
                            { "Status",      e.IsActive == true ? "Active" : "Inactive" }
                        });
                    break;

                case "EMP-ATTENDANCE":
                    r.ReportName = "Attendance Report";
                    r.Columns = new List<string> { "Employee", "Date", "Check In", "Check Out", "Status", "Overtime" };

                    var atts = await context.EmployeeAttendances
                        .Where(a => a.IsDeleted != true)
                        .OrderByDescending(a => a.AttendanceDate)
                        .Take(500)
                        .ToListAsync();

                    var attEmpIds = atts.Select(a => a.EmployeeId).Distinct().ToList();
                    var attEmpLookup = await context.Employees
                        .Where(e => attEmpIds.Contains(e.EmployeeId))
                        .ToDictionaryAsync(
                            e => e.EmployeeId,
                            e => (e.FirstName + " " + (e.LastName ?? "")).Trim());

                    foreach (var a in atts)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Employee",  attEmpLookup.GetValueOrDefault(a.EmployeeId, "-") },
                            { "Date",      $"{a.AttendanceDate:dd MMM yyyy}" },
                            { "Check In",  a.CheckInTime.HasValue  ? $"{a.CheckInTime.Value:HH\\:mm}"  : "-" },
                            { "Check Out", a.CheckOutTime.HasValue ? $"{a.CheckOutTime.Value:HH\\:mm}" : "-" },
                            { "Status",    a.Status ?? "Present" },
                            { "Overtime",  $"{(a.OvertimeHours ?? 0)} hrs" }
                        });
                    break;

                case "EMP-LEAVE":
                    r.ReportName = "Leave Report";
                    r.Columns = new List<string> { "Employee", "Type", "From", "To", "Days", "Status" };

                    var leaves = await context.EmployeeLeaves
                        .Include(l => l.Employee)
                        .Where(l => l.IsDeleted != true)
                        .OrderByDescending(l => l.FromDate)
                        .Take(500)
                        .ToListAsync();

                    foreach (var l in leaves)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Employee", l.Employee != null ? (l.Employee.FirstName + " " + (l.Employee.LastName ?? "")).Trim() : "-" },
                            { "Type",     l.LeaveType ?? "Casual" },
                            { "From",     $"{l.FromDate:dd MMM yyyy}" },
                            { "To",       $"{l.ToDate:dd MMM yyyy}" },
                            { "Days",     l.TotalDays },
                            { "Status",   l.Status ?? "Pending" }
                        });
                    break;

                case "EMP-SALARY":
                    r.ReportName = "Salary Report";
                    r.Columns = new List<string> { "Employee", "Basic", "Allowances", "Deductions", "Net", "Effective" };

                    var sals = await context.EmployeeSalaries
                        .Include(s => s.Employee)
                        .Where(s => s.IsActive != false)
                        .OrderByDescending(s => s.EffectiveDate)
                        .Take(500)
                        .ToListAsync();

                    foreach (var s in sals)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Employee",   s.Employee != null ? (s.Employee.FirstName + " " + (s.Employee.LastName ?? "")).Trim() : "-" },
                            { "Basic",      $"Rs {s.BasicSalary:N0}" },
                            { "Allowances", $"Rs {(s.TotalAllowances ?? 0):N0}" },
                            { "Deductions", $"Rs {(s.TotalDeductions ?? 0):N0}" },
                            { "Net",        $"Rs {(s.NetSalary ?? 0):N0}" },
                            { "Effective",  $"{s.EffectiveDate:dd MMM yyyy}" }
                        });
                    break;

                case "EMP-PAYROLL":
                    r.ReportName = "Payroll Report";
                    r.Columns = new List<string> { "Employee", "Month", "Gross", "Deductions", "Net Pay", "Status" };

                    var payrolls = await context.EmployeePayrolls
                        .Include(p => p.Employee)
                        .OrderByDescending(p => p.MonthYear)
                        .Take(500)
                        .ToListAsync();

                    foreach (var p in payrolls)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Employee",   p.Employee != null ? (p.Employee.FirstName + " " + (p.Employee.LastName ?? "")).Trim() : "-" },
                            // ✅ String interpolation - string, DateOnly, DateTime sab ke liye kaam karta hai
                            { "Month",      $"{p.MonthYear:MMM yyyy}" },
                            { "Gross",      $"Rs {p.GrossPay:N0}" },
                            { "Deductions", $"Rs {(p.TotalDeductions ?? 0):N0}" },
                            { "Net Pay",    $"Rs {p.NetPay:N0}" },
                            { "Status",     p.Status ?? "Draft" }
                        });
                    break;

                case "INV-STOCK-LEDGER":
                    r.ReportName = "Stock Ledger";
                    r.Columns = new List<string> { "Product", "Warehouse", "Type", "Qty", "Balance", "Date" };

                    var sl = await context.StockLedgers
                        .Include(s => s.Product)
                        .Include(s => s.Warehouse)
                        .Where(s => s.IsDeleted != true)
                        .OrderByDescending(s => s.LedgerId)
                        .Take(500)
                        .ToListAsync();

                    foreach (var s in sl)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Product",   s.Product != null ? s.Product.ProductName : "-" },
                            { "Warehouse", s.Warehouse != null ? s.Warehouse.WarehouseName : "-" },
                            { "Type",      s.TransactionType ?? "-" },
                            { "Qty",       s.Quantity },
                            { "Balance",   s.Balance },
                            { "Date",      s.CreatedDate.HasValue ? $"{s.CreatedDate.Value:dd MMM yyyy}" : "-" }
                        });
                    break;

                case "SALES-INVOICE":
                    r.ReportName = "Invoice Report";
                    r.Columns = new List<string> { "Invoice No", "Date", "Net Amount", "Paid", "Balance", "Status" };

                    var inv = await context.Invoices
                        .Where(i => i.IsDeleted != true)
                        .OrderByDescending(i => i.InvoiceId)
                        .Take(500)
                        .ToListAsync();

                    foreach (var i in inv)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Invoice No", i.InvoiceNo ?? "-" },
                            { "Date",       i.InvoiceDate.HasValue ? $"{i.InvoiceDate.Value:dd MMM yyyy}" : "-" },
                            { "Net Amount", $"Rs {i.NetAmount:N0}" },
                            { "Paid",       $"Rs {(i.PaidAmount ?? 0):N0}" },
                            { "Balance",    $"Rs {i.BalanceDue:N0}" },
                            { "Status",     i.Status ?? "Pending" }
                        });
                    break;

                case "CUST-LIST":
                    r.ReportName = "Customer List";
                    r.Columns = new List<string> { "Name", "Phone", "Email", "City", "Status" };

                    var cust = await context.Customers
                        .Where(c => c.IsDeleted != true)
                        .Take(500)
                        .ToListAsync();

                    foreach (var c in cust)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Name",   c.CustomerName ?? "-" },
                            { "Phone",  c.Phone ?? "-" },
                            { "Email",  c.Email ?? "-" },
                            { "City",   c.City ?? "-" },
                            { "Status", c.IsActive == true ? "Active" : "Inactive" }
                        });
                    break;

                case "SUPP-LIST":
                    r.ReportName = "Supplier List";
                    r.Columns = new List<string> { "Name", "Phone", "Email", "City", "Status" };

                    var supp = await context.Suppliers
                        .Where(s => s.IsDeleted != true)
                        .Take(500)
                        .ToListAsync();

                    foreach (var s in supp)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Name",   s.SupplierName ?? "-" },
                            { "Phone",  s.Phone ?? "-" },
                            { "Email",  s.Email ?? "-" },
                            { "City",   s.City ?? "-" },
                            { "Status", s.IsActive == true ? "Active" : "Inactive" }
                        });
                    break;

                case "PROD-LIST":
                    r.ReportName = "Product List";
                    r.Columns = new List<string> { "Name", "Category", "Unit", "Status" };

                    var prod = await context.Products
                        .Include(p => p.Category)
                        .Where(p => p.IsDeleted != true)
                        .Take(500)
                        .ToListAsync();

                    foreach (var p in prod)
                        r.Rows.Add(new Dictionary<string, object>
                        {
                            { "Name",     p.ProductName ?? "-" },
                            { "Category", p.Category != null ? p.Category.CategoryName : "-" },
                            { "Unit",     p.UnitId.ToString() },
                            { "Status",   p.IsActive == true ? "Active" : "Inactive" }
                        });
                    break;

                default:
                    r.ReportName = "Report: " + code;
                    r.Columns = new List<string> { "Message" };
                    r.Rows.Add(new Dictionary<string, object> { { "Message", "Report not implemented yet." } });
                    break;
            }

            r.TotalRows = r.Rows.Count;
            return r;
        }

        // =========================================================
        // EXPORT TO EXCEL
        // =========================================================
        public byte[] ExportToExcel(ReportResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", result.Columns));

            foreach (var row in result.Rows)
                sb.AppendLine(string.Join("\t",
                    result.Columns.Select(c =>
                        row.TryGetValue(c, out var val) ? val?.ToString() ?? "" : "")));

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        // =========================================================
        // EXPORT TO PDF
        // =========================================================
        public byte[] ExportToPdf(ReportResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine(result.ReportName);
            sb.AppendLine(string.Join(" | ", result.Columns));

            foreach (var row in result.Rows)
                sb.AppendLine(string.Join(" | ",
                    result.Columns.Select(c =>
                        row.TryGetValue(c, out var val) ? val?.ToString() ?? "" : "")));

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        // =========================================================
        // EXPORT TO CSV
        // =========================================================
        public string ExportToCsv(ReportResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", result.Columns));

            foreach (var row in result.Rows)
                sb.AppendLine(string.Join(",",
                    result.Columns.Select(c =>
                    {
                        var val = row.TryGetValue(c, out var v) ? v?.ToString() ?? "" : "";
                        return "\"" + val.Replace("\"", "\"\"") + "\"";
                    })));

            return sb.ToString();
        }
    }
}