#nullable disable
using ERPWMS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.Reports
{
    [Authorize(Policy = "Reports.Access")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        public ReportsController(AppDbContext context) { _context = context; }

        private static readonly HashSet<string> AdminOnlyReports = new(StringComparer.OrdinalIgnoreCase)
        {
            "SYSTEMSETTING", "BACKUP", "AUDITLOG", "USER"
        };

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Viewer(string code)
        {
            if (string.IsNullOrEmpty(code)) return RedirectToAction("Index");
            if (!IsAllowed(code)) return Forbid();
            ViewBag.ReportCode = code;
            ViewBag.ReportName = GetReportName(code);
            return View();
        }

        private bool IsAllowed(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            if (AdminOnlyReports.Contains(code))
                return User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceSummary(int year, int month)
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
                return Json(new { success = false, message = "Invalid date range." });

            var start = new DateOnly(year, month, 1);
            var end = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var data = await (from e in _context.Employees
                              where e.IsDeleted != true && e.IsActive == true
                              orderby e.EmployeeCode
                              let att = _context.EmployeeAttendances
                                  .Where(a => a.EmployeeId == e.EmployeeId
                                      && a.AttendanceDate >= start
                                      && a.AttendanceDate <= end)
                              select new
                              {
                                  code = e.EmployeeCode ?? "",
                                  employee = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                                  present = att.Count(a => a.Status != "Absent"),
                                  absent = att.Count(a => a.Status == "Absent"),
                                  late = att.Count(a => (a.LateMinutes ?? 0) > 0),
                                  otHours = att.Sum(a => (decimal?)a.OTHours) ?? 0m
                              }).ToListAsync();

            return Json(new { success = true, year, month, data });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(string code, DateTime? FromDate, DateTime? ToDate)
        {
            if (!IsAllowed(code))
                return Forbid();

            var reportName = GetReportName(code);
            var rows = new List<Dictionary<string, object>>();
            var columns = new List<string>();
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            try
            {
                switch (code?.ToUpper())
                {
                    case "COMPANY":
                        columns = new List<string> { "ID", "Company Name", "Registration No", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Companies.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.CompanyId, ["Company Name"] = x.CompanyName ?? "",
                                ["Registration No"] = x.RegistrationNo ?? "", ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "BRANCH":
                        columns = new List<string> { "ID", "Branch Name", "Code", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Branches.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.BranchId, ["Branch Name"] = x.BranchName ?? "",
                                ["Code"] = x.BranchCode ?? "", ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "DEPARTMENT":
                        columns = new List<string> { "ID", "Department Name", "Code", "Status" };
                        foreach (var x in await _context.Departments.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.DepartmentId, ["Department Name"] = x.DepartmentName ?? "",
                                ["Code"] = x.DepartmentCode ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "WAREHOUSE":
                        columns = new List<string> { "ID", "Warehouse Name", "Code", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Warehouses.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.WarehouseId, ["Warehouse Name"] = x.WarehouseName ?? "",
                                ["Code"] = x.WarehouseCode ?? "", ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "CUSTOMER":
                        columns = new List<string> { "ID", "Customer Name", "Code", "Contact Person", "Phone", "Email", "City", "Status" };
                        foreach (var x in await _context.Customers.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.CustomerId, ["Customer Name"] = x.CustomerName ?? "",
                                ["Code"] = x.CustomerCode ?? "", ["Contact Person"] = x.ContactPerson ?? "",
                                ["Phone"] = x.Phone ?? "", ["Email"] = x.Email ?? "",
                                ["City"] = x.City ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "SUPPLIER":
                        columns = new List<string> { "ID", "Supplier Name", "Code", "Contact Person", "Phone", "Email", "City", "Status" };
                        foreach (var x in await _context.Suppliers.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.SupplierId, ["Supplier Name"] = x.SupplierName ?? "",
                                ["Code"] = x.SupplierCode ?? "", ["Contact Person"] = x.ContactPerson ?? "",
                                ["Phone"] = x.Phone ?? "", ["Email"] = x.Email ?? "",
                                ["City"] = x.City ?? "", ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "PRODUCT":
                        columns = new List<string> { "ID", "Product Name", "SKU", "Barcode", "Min Stock", "Max Stock", "Status" };
                        foreach (var x in await _context.Products.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.ProductId, ["Product Name"] = x.ProductName ?? "",
                                ["SKU"] = x.Sku ?? "", ["Barcode"] = x.Barcode ?? "",
                                ["Min Stock"] = x.MinStockLevel.ToString(), ["Max Stock"] = x.MaxStockLevel.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "SALES":
                        columns = new List<string> { "ID", "SO No", "Order Date", "Total Amount", "Net Amount", "Status" };
                        foreach (var x in await _context.SalesMasters.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.SalesId, ["SO No"] = x.SalesOrderNo ?? "",
                                ["Order Date"] = x.OrderDate.ToString(),
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Net Amount"] = x.NetAmount.ToString("N2"), ["Status"] = x.Status ?? ""
                            });
                        break;

                    case "INVOICE":
                        columns = new List<string> { "ID", "Invoice No", "Invoice Date", "Total Amount", "Paid", "Balance", "Status" };
                        foreach (var x in await _context.Invoices.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.InvoiceId, ["Invoice No"] = x.InvoiceNo ?? "",
                                ["Invoice Date"] = x.InvoiceDate.ToString(),
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Paid"] = (x.PaidAmount ?? 0).ToString("N2"),
                                ["Balance"] = (x.BalanceDue ?? 0).ToString("N2"), ["Status"] = x.Status ?? ""
                            });
                        break;

                    case "STOCKLEDGER":
                        columns = new List<string> { "ID", "Transaction Type", "Quantity", "Balance", "Unit Cost", "Date" };
                        foreach (var x in await _context.StockLedgers.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.LedgerId, ["Transaction Type"] = x.TransactionType ?? "",
                                ["Quantity"] = x.Quantity, ["Balance"] = x.Balance,
                                ["Unit Cost"] = x.UnitCost.ToString("N2"), ["Date"] = x.TransactionDate.ToString()
                            });
                        break;

                    case "EMPLOYEE":
                        columns = new List<string> { "ID", "Code", "First Name", "Last Name", "CNIC", "Gender", "Status" };
                        foreach (var x in await _context.Employees.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.EmployeeId, ["Code"] = x.EmployeeCode ?? "",
                                ["First Name"] = x.FirstName ?? "", ["Last Name"] = x.LastName ?? "",
                                ["CNIC"] = isAdmin ? (x.Cnic ?? "") : MaskCnic(x.Cnic),
                                ["Gender"] = x.Gender ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    case "PAYROLL":
                        columns = new List<string> { "ID", "Employee ID", "Month", "Gross Pay", "Deductions", "Net Pay", "Status" };
                        foreach (var x in await _context.EmployeePayrolls.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.PayrollId, ["Employee ID"] = x.EmployeeId,
                                ["Month"] = x.MonthYear.ToString(),
                                ["Gross Pay"] = x.GrossPay.ToString("N2"),
                                ["Deductions"] = (x.TotalDeductions ?? 0).ToString("N2"),
                                ["Net Pay"] = x.NetPay.ToString("N2"), ["Status"] = x.Status ?? ""
                            });
                        break;

                    // ===== BLOCKED FOR NON-ADMIN =====
                    case "SYSTEMSETTING":
                        return Forbid();

                    case "USER":
                        if (!isAdmin) return Forbid();
                        columns = new List<string> { "ID", "Username", "Email", "Phone", "Status" };
                        foreach (var x in await _context.Users.AsNoTracking().ToListAsync())
                            rows.Add(new Dictionary<string, object> {
                                ["ID"] = x.Id, ["Username"] = x.UserName ?? "",
                                ["Email"] = x.Email ?? "", ["Phone"] = x.PhoneNumber ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        break;

                    default:
                        return Json(new { success = false, message = "Unknown report code." });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Report generation failed." });
            }

            var result = new ERPWMS.Application.DTOs.Reports.ReportResult
            {
                ReportCode = code ?? "",
                ReportName = reportName,
                Columns = columns,
                Rows = rows,
                TotalRows = rows.Count,
                GeneratedAt = DateTime.Now
            };
            return PartialView("_ReportTable", result);
        }

        [HttpGet]
        public IActionResult ExportExcel(string code)
        {
            if (!IsAllowed(code)) return Forbid();
            return StatusCode(501, "Excel export not yet implemented.");
        }

        [HttpGet]
        public IActionResult ExportCsv(string code)
        {
            if (!IsAllowed(code)) return Forbid();
            return StatusCode(501, "CSV export not yet implemented.");
        }

        private static string MaskCnic(string cnic)
        {
            if (string.IsNullOrEmpty(cnic) || cnic.Length < 5) return "***";
            return cnic.Substring(0, 2) + "*****" + cnic.Substring(cnic.Length - 3);
        }

        private static string GetReportName(string code) => code?.ToUpper() switch
        {
            "COMPANY" => "Companies", "BRANCH" => "Branches", "DEPARTMENT" => "Departments",
            "WAREHOUSE" => "Warehouses", "CUSTOMER" => "Customers", "SUPPLIER" => "Suppliers",
            "PRODUCT" => "Products", "SALES" => "Sales Orders", "INVOICE" => "Invoices",
            "STOCKLEDGER" => "Stock Ledger", "EMPLOYEE" => "Employees", "PAYROLL" => "Payroll",
            "SYSTEMSETTING" => "System Settings", "USER" => "Users",
            _ => code ?? "Report"
        };
    }
}
