#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Reports
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Viewer(string code)
        {
            if (string.IsNullOrEmpty(code)) return RedirectToAction("Index");
            ViewBag.ReportCode = code;
            ViewBag.ReportName = GetReportName(code);
            return View();
        }

        // ============ ATTENDANCE SUMMARY — month/year filter ============
        [HttpGet]
        public async Task<IActionResult> GetAttendanceSummary(int year, int month)
        {
            try
            {
                var start = new DateOnly(year, month, 1);
                var end = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

                var employees = await _context.Employees
                    .Where(e => e.IsDeleted != true && e.IsActive == true)
                    .OrderBy(e => e.EmployeeCode)
                    .ToListAsync();

                var result = new List<object>();
                foreach (var e in employees)
                {
                    var att = await _context.EmployeeAttendances
                        .Where(a => a.EmployeeId == e.EmployeeId
                            && a.AttendanceDate >= start
                            && a.AttendanceDate <= end)
                        .ToListAsync();

                    result.Add(new
                    {
                        code = e.EmployeeCode ?? "",
                        employee = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                        present = att.Count(a => a.Status != "Absent"),
                        absent = att.Count(a => a.Status == "Absent"),
                        late = att.Count(a => (a.LateMinutes ?? 0) > 0),
                        otHours = att.Sum(a => a.OTHours ?? 0)
                    });
                }

                return Json(new { success = true, year = year, month = month, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(string code, DateTime? FromDate, DateTime? ToDate)
        {
            _ = FromDate; _ = ToDate;

            var reportName = GetReportName(code);
            var rows = new List<Dictionary<string, object>>();
            var columns = new List<string>();

            try
            {
                switch (code?.ToUpper())
                {
                    case "COMPANY":
                        columns = new List<string> { "ID", "Company Name", "Registration No", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Companies.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.CompanyId,
                                ["Company Name"] = x.CompanyName ?? "",
                                ["Registration No"] = x.RegistrationNo ?? "",
                                ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "BRANCH":
                        columns = new List<string> { "ID", "Branch Name", "Code", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Branches.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BranchId,
                                ["Branch Name"] = x.BranchName ?? "",
                                ["Code"] = x.BranchCode ?? "",
                                ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "DEPARTMENT":
                        columns = new List<string> { "ID", "Department Name", "Code", "Status" };
                        foreach (var x in await _context.Departments.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.DepartmentId,
                                ["Department Name"] = x.DepartmentName ?? "",
                                ["Code"] = x.DepartmentCode ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "DESIGNATION":
                        columns = new List<string> { "ID", "Designation", "Rank Level", "Status" };
                        foreach (var x in await _context.Designations.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.DesignationId,
                                ["Designation"] = x.DesignationName ?? "",
                                ["Rank Level"] = x.RankLevel.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "WAREHOUSE":
                        columns = new List<string> { "ID", "Warehouse Name", "Code", "Phone", "Email", "Status" };
                        foreach (var x in await _context.Warehouses.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.WarehouseId,
                                ["Warehouse Name"] = x.WarehouseName ?? "",
                                ["Code"] = x.WarehouseCode ?? "",
                                ["Phone"] = x.Phone ?? "",
                                ["Email"] = x.Email ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "ZONE":
                        columns = new List<string> { "ID", "Zone Name", "Code", "Warehouse", "Status" };
                        foreach (var x in await _context.Zones.Include(z => z.Warehouse).ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ZoneId,
                                ["Zone Name"] = x.ZoneName ?? "",
                                ["Code"] = x.ZoneCode ?? "",
                                ["Warehouse"] = x.Warehouse != null ? x.Warehouse.WarehouseName : "-",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "RACK":
                        columns = new List<string> { "ID", "Rack Name", "Code", "Zone", "Status" };
                        foreach (var x in await _context.Racks.Include(z => z.Zone).ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.RackId,
                                ["Rack Name"] = x.RackName ?? "",
                                ["Code"] = x.RackCode ?? "",
                                ["Zone"] = x.Zone != null ? x.Zone.ZoneName : "-",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "BIN":
                        columns = new List<string> { "ID", "Bin Name", "Code", "Rack", "Status" };
                        foreach (var x in await _context.Bins.Include(z => z.Rack).ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BinId,
                                ["Bin Name"] = x.BinName ?? "",
                                ["Code"] = x.BinCode ?? "",
                                ["Rack"] = x.Rack != null ? x.Rack.RackName : "-",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "CUSTOMER":
                        columns = new List<string> { "ID", "Customer Name", "Code", "Contact Person", "Phone", "Mobile", "Email", "City", "Status" };
                        foreach (var x in await _context.Customers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.CustomerId,
                                ["Customer Name"] = x.CustomerName ?? "",
                                ["Code"] = x.CustomerCode ?? "",
                                ["Contact Person"] = x.ContactPerson ?? "",
                                ["Phone"] = x.Phone ?? "",
                                ["Mobile"] = x.Mobile ?? "",
                                ["Email"] = x.Email ?? "",
                                ["City"] = x.City ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "SUPPLIER":
                        columns = new List<string> { "ID", "Supplier Name", "Code", "Contact Person", "Phone", "Mobile", "Email", "City", "Status" };
                        foreach (var x in await _context.Suppliers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SupplierId,
                                ["Supplier Name"] = x.SupplierName ?? "",
                                ["Code"] = x.SupplierCode ?? "",
                                ["Contact Person"] = x.ContactPerson ?? "",
                                ["Phone"] = x.Phone ?? "",
                                ["Mobile"] = x.Mobile ?? "",
                                ["Email"] = x.Email ?? "",
                                ["City"] = x.City ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "CATEGORY":
                        columns = new List<string> { "ID", "Category Name", "Code", "Description", "Status" };
                        foreach (var x in await _context.Categories.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.CategoryId,
                                ["Category Name"] = x.CategoryName ?? "",
                                ["Code"] = x.CategoryCode ?? "",
                                ["Description"] = x.Description ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "SUBCATEGORY":
                        columns = new List<string> { "ID", "Sub Category", "Code", "Category", "Status" };
                        foreach (var x in await _context.SubCategories.Include(s => s.Category).ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SubCategoryId,
                                ["Sub Category"] = x.SubCategoryName ?? "",
                                ["Code"] = x.SubCategoryCode ?? "",
                                ["Category"] = x.Category != null ? x.Category.CategoryName : "-",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "BRAND":
                        columns = new List<string> { "ID", "Brand Name", "Code", "Status" };
                        foreach (var x in await _context.Brands.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BrandId,
                                ["Brand Name"] = x.BrandName ?? "",
                                ["Code"] = x.BrandCode ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "UNIT":
                        columns = new List<string> { "ID", "Unit Name", "Code", "Conversion", "Status" };
                        foreach (var x in await _context.Units.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.UnitId,
                                ["Unit Name"] = x.UnitName ?? "",
                                ["Code"] = x.UnitCode ?? "",
                                ["Conversion"] = x.ConversionFactor.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "PRODUCT":
                        columns = new List<string> { "ID", "Product Name", "SKU", "Barcode", "Min Stock", "Max Stock", "Status" };
                        foreach (var x in await _context.Products.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ProductId,
                                ["Product Name"] = x.ProductName ?? "",
                                ["SKU"] = x.Sku ?? "",
                                ["Barcode"] = x.Barcode ?? "",
                                ["Min Stock"] = x.MinStockLevel.ToString(),
                                ["Max Stock"] = x.MaxStockLevel.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "PURCHASEMASTER":
                        columns = new List<string> { "ID", "PO No", "Order Date", "Total Amount", "Net Amount", "Status" };
                        foreach (var x in await _context.PurchaseMasters.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.PurchaseId,
                                ["PO No"] = x.PurchaseOrderNo ?? "",
                                ["Order Date"] = x.OrderDate.ToString(),
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Net Amount"] = x.NetAmount.ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "GRN":
                        columns = new List<string> { "ID", "Grn No", "Receipt Date", "Total Qty", "Accepted", "Status" };
                        foreach (var x in await _context.Grns.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.GrnId,
                                ["Grn No"] = x.GrnNo ?? "",
                                ["Receipt Date"] = x.ReceiptDate.ToString(),
                                ["Total Qty"] = x.TotalReceivedQty,
                                ["Accepted"] = x.TotalAcceptedQty,
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "PURCHASERETURN":
                        columns = new List<string> { "ID", "Return No", "Return Date", "Reason", "Total Amount", "Status" };
                        foreach (var x in await _context.PurchaseReturns.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ReturnId,
                                ["Return No"] = x.ReturnNo ?? "",
                                ["Return Date"] = x.ReturnDate.ToString(),
                                ["Reason"] = x.ReturnReason ?? "",
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "SALES":
                        columns = new List<string> { "ID", "SO No", "Order Date", "Total Amount", "Net Amount", "Status" };
                        foreach (var x in await _context.SalesMasters.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SalesId,
                                ["SO No"] = x.SalesOrderNo ?? "",
                                ["Order Date"] = x.OrderDate.ToString(),
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Net Amount"] = x.NetAmount.ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "INVOICE":
                        columns = new List<string> { "ID", "Invoice No", "Invoice Date", "Total Amount", "Paid", "Balance", "Status" };
                        foreach (var x in await _context.Invoices.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.InvoiceId,
                                ["Invoice No"] = x.InvoiceNo ?? "",
                                ["Invoice Date"] = x.InvoiceDate.ToString(),
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),

                                
                                ["Paid"] = (x.PaidAmount ?? 0).ToString("N2"),
                                ["Balance"] = (x.BalanceDue ?? 0).ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "DISPATCH":
                        columns = new List<string> { "ID", "Dispatch No", "Dispatch Date", "Carrier", "Tracking No", "Status" };
                        foreach (var x in await _context.DispatchMasters.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.DispatchId,
                                ["Dispatch No"] = x.DispatchNo ?? "",
                                ["Dispatch Date"] = x.DispatchDate.ToString(),
                                ["Carrier"] = x.CarrierName ?? "",
                                ["Tracking No"] = x.TrackingNo ?? "",
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "PICKINGLIST":
                        columns = new List<string> { "ID", "Picking No", "Picking Date", "Assigned To", "Picked By", "Status" };
                        foreach (var x in await _context.PickingLists.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.PickingId,
                                ["Picking No"] = x.PickingNo ?? "",
                                ["Picking Date"] = x.PickingDate.ToString(),
                                ["Assigned To"] = x.AssignedTo.ToString(),
                                ["Picked By"] = x.PickedBy.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "PICKING":
                        columns = new List<string> { "ID", "Picked Qty", "Batch No", "Status" };
                        foreach (var x in await _context.PickingDetails.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.PickingDetailId,
                                ["Picked Qty"] = x.PickedQty,
                                ["Batch No"] = x.BatchNo ?? "",
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "SALESRETURN":
                        columns = new List<string> { "ID", "Return No", "Return Date", "Reason", "Total Amount", "Status" };
                        foreach (var x in await _context.SalesReturns.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ReturnId,
                                ["Return No"] = x.ReturnNo ?? "",
                                ["Return Date"] = x.ReturnDate.ToString(),
                                ["Reason"] = x.ReturnReason ?? "",
                                ["Total Amount"] = x.TotalAmount.ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "STOCKLEDGER":
                        columns = new List<string> { "ID", "Transaction Type", "Quantity", "Balance", "Unit Cost", "Date" };
                        foreach (var x in await _context.StockLedgers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.LedgerId,
                                ["Transaction Type"] = x.TransactionType ?? "",
                                ["Quantity"] = x.Quantity,
                                ["Balance"] = x.Balance,
                                ["Unit Cost"] = x.UnitCost.ToString("N2"),
                                ["Date"] = x.TransactionDate.ToString()
                            });
                        }
                        break;

                    case "STOCKADJUSTMENT":
                    case "ADJUSTMENT":
                        columns = new List<string> { "ID", "Type", "Quantity", "New Qty", "Reason", "Status" };
                        foreach (var x in await _context.StockAdjustments.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.AdjustmentId,
                                ["Type"] = x.AdjustmentType ?? "",
                                ["Quantity"] = x.Quantity,
                                ["New Qty"] = x.NewQty,
                                ["Reason"] = x.Reason ?? "",
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "STOCKTRANSFER":
                    case "TRANSFER":
                        columns = new List<string> { "ID", "Transfer No", "From WH", "To WH", "Quantity", "Date", "Status" };
                        foreach (var x in await _context.StockTransfers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.TransferId,
                                ["Transfer No"] = x.TransferNo ?? "",
                                ["From WH"] = x.FromWarehouseId,
                                ["To WH"] = x.ToWarehouseId,
                                ["Quantity"] = x.Quantity,
                                ["Date"] = x.TransferDate.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "CYCLECOUNT":
                        columns = new List<string> { "ID", "System Qty", "Actual Qty", "Variance", "Counted Date", "Status" };
                        foreach (var x in await _context.CycleCounts.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.CycleCountId,
                                ["System Qty"] = x.SystemQty,
                                ["Actual Qty"] = x.ActualQty,
                                ["Variance"] = x.Variance,
                                ["Counted Date"] = x.CountedDate.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "BATCHTRACKING":
                        columns = new List<string> { "ID", "Batch No", "Expiry Date", "Quantity", "Remaining", "Status" };
                        foreach (var x in await _context.BatchTrackings.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BatchId,
                                ["Batch No"] = x.BatchNo ?? "",
                                ["Expiry Date"] = x.ExpiryDate.ToString(),
                                ["Quantity"] = x.Quantity,
                                ["Remaining"] = x.RemainingQty,
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "SERIALNUMBER":
                        columns = new List<string> { "ID", "Serial No", "Status", "Out Date" };
                        foreach (var x in await _context.SerialNumbers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SerialId,
                                ["Serial No"] = x.SerialNo ?? "",
                                ["Status"] = x.Status ?? "",
                                ["Out Date"] = x.OutDate.ToString()
                            });
                        }
                        break;

                    case "CHARTOFACCOUNT":
                        columns = new List<string> { "ID", "Account Code", "Account Name", "Type", "Status" };
                        foreach (var x in await _context.ChartOfAccounts.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.AccountId,
                                ["Account Code"] = x.AccountCode ?? "",
                                ["Account Name"] = x.AccountName ?? "",
                                ["Type"] = x.AccountType ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "JOURNALVOUCHER":
                        columns = new List<string> { "ID", "Voucher No", "Date", "Total Debit", "Total Credit", "Status" };
                        foreach (var x in await _context.JournalVouchers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.VoucherId,
                                ["Voucher No"] = x.VoucherNo ?? "",
                                ["Date"] = x.VoucherDate.ToString(),
                                ["Total Debit"] = x.TotalDebit.ToString(),
                                ["Total Credit"] = x.TotalCredit.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "PAYMENTVOUCHER":
                        columns = new List<string> { "ID", "Payment No", "Date", "Amount", "Method", "Status" };
                        foreach (var x in await _context.PaymentVouchers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.PaymentId,
                                ["Payment No"] = x.PaymentNo ?? "",
                                ["Date"] = x.PaymentDate.ToString(),
                                ["Amount"] = x.Amount.ToString(),
                                ["Method"] = x.PaymentMethod ?? "",
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "RECEIPTVOUCHER":
                        columns = new List<string> { "ID", "Receipt No", "Date", "Amount", "Method", "Status" };
                        foreach (var x in await _context.ReceiptVouchers.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ReceiptId,
                                ["Receipt No"] = x.ReceiptNo ?? "",
                                ["Date"] = x.ReceiptDate.ToString(),
                                ["Amount"] = x.Amount.ToString(),
                                ["Method"] = x.PaymentMethod ?? "",
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "BANKACCOUNT":
                        columns = new List<string> { "ID", "Bank", "Account Title", "Account No", "IBAN", "Status" };
                        foreach (var x in await _context.BankAccounts.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BankAccountId,
                                ["Bank"] = x.BankName ?? "",
                                ["Account Title"] = x.AccountTitle ?? "",
                                ["Account No"] = x.AccountNo ?? "",
                                ["IBAN"] = x.Iban ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "BANKRECONCILIATION":
                        columns = new List<string> { "ID", "Statement Date", "Statement Balance", "System Balance", "Difference", "Status" };
                        foreach (var x in await _context.BankReconciliations.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ReconciliationId,
                                ["Statement Date"] = x.StatementDate.ToString(),
                                ["Statement Balance"] = x.StatementBalance.ToString(),
                                ["System Balance"] = x.SystemBalance.ToString(),
                                ["Difference"] = x.Difference.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "TAXSETTING":
                        columns = new List<string> { "ID", "Tax Name", "Code", "Rate", "Status" };
                        foreach (var x in await _context.TaxSettings.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.TaxId,
                                ["Tax Name"] = x.TaxName ?? "",
                                ["Code"] = x.TaxCode ?? "",
                                ["Rate"] = x.TaxRate.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "EMPLOYEE":
                        columns = new List<string> { "ID", "Code", "First Name", "Last Name", "CNIC", "Gender", "Base Salary", "Status" };
                        foreach (var x in await _context.Employees.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.EmployeeId,
                                ["Code"] = x.EmployeeCode ?? "",
                                ["First Name"] = x.FirstName ?? "",
                                ["Last Name"] = x.LastName ?? "",
                                ["CNIC"] = x.Cnic ?? "",
                                ["Gender"] = x.Gender ?? "",
                                ["Base Salary"] = x.BaseSalary.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "ATTENDANCE":
                        columns = new List<string> { "ID", "Employee ID", "Date", "Check In", "Check Out", "Status" };
                        foreach (var x in await _context.EmployeeAttendances.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.AttendanceId,
                                ["Employee ID"] = x.EmployeeId,
                                ["Date"] = x.AttendanceDate.ToString(),
                                ["Check In"] = x.CheckInTime.ToString(),
                                ["Check Out"] = x.CheckOutTime.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "LEAVE":
                        columns = new List<string> { "ID", "Employee ID", "Leave Type", "From", "To", "Days", "Status" };
                        foreach (var x in await _context.EmployeeLeaves.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.LeaveId,
                                ["Employee ID"] = x.EmployeeId,
                                ["Leave Type"] = x.LeaveType ?? "",
                                ["From"] = x.FromDate.ToString(),
                                ["To"] = x.ToDate.ToString(),
                                ["Days"] = x.TotalDays,
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "PAYROLL":
                        columns = new List<string> { "ID", "Employee ID", "Month", "Gross Pay", "Deductions", "Net Pay", "Status" };
                        foreach (var x in await _context.EmployeePayrolls.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.PayrollId,
                                ["Employee ID"] = x.EmployeeId,
                                ["Month"] = x.MonthYear.ToString(),
                                ["Gross Pay"] = x.GrossPay.ToString(),
                                ["Deductions"] = x.TotalDeductions.ToString(),
                                ["Net Pay"] = x.NetPay.ToString(),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    case "EMPLOYEEBANKACCOUNT":
                        columns = new List<string> { "ID", "Employee ID", "Bank", "Account No", "IBAN", "Primary" };
                        foreach (var x in await _context.EmployeeBankAccounts.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BankAccountId,
                                ["Employee ID"] = x.EmployeeId,
                                ["Bank"] = x.BankName ?? "",
                                ["Account No"] = x.AccountNumber ?? "",
                                ["IBAN"] = x.Iban ?? "",
                                ["Primary"] = x.IsPrimary == true ? "Yes" : "No"
                            });
                        }
                        break;

                    case "EMPLOYEESALARY":
                        columns = new List<string> { "ID", "Employee ID", "Basic", "Allowances", "Deductions", "Net Salary", "Status" };
                        foreach (var x in await _context.EmployeeSalaries.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SalaryId,
                                ["Employee ID"] = x.EmployeeId,
                                ["Basic"] = x.BasicSalary.ToString(),
                                ["Allowances"] = x.TotalAllowances.ToString(),
                                ["Deductions"] = x.TotalDeductions.ToString(),
                                ["Net Salary"] = x.NetSalary.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "SHIFT":
                        columns = new List<string> { "ID", "Shift Name", "Start", "End", "Grace Min", "Status" };
                        foreach (var x in await _context.Shifts.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.ShiftId,
                                ["Shift Name"] = x.ShiftName ?? "",
                                ["Start"] = x.StartTime.ToString(),
                                ["End"] = x.EndTime.ToString(),
                                ["Grace Min"] = x.GracePeriodMinutes.ToString(),
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "USER":
                        columns = new List<string> { "ID", "Username", "Email", "Phone", "Status" };
                        foreach (var x in await _context.Users.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.Id,
                                ["Username"] = x.UserName ?? "",
                                ["Email"] = x.Email ?? "",
                                ["Phone"] = x.PhoneNumber ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "SYSTEMSETTING":
                        columns = new List<string> { "ID", "Key", "Value", "Type" };
                        foreach (var x in await _context.SystemSettings.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.SettingId,
                                ["Key"] = x.SettingKey ?? "",
                                ["Value"] = x.SettingValue ?? "",
                                ["Type"] = x.DataType ?? ""
                            });
                        }
                        break;

                    case "NOTIFICATION":
                        columns = new List<string> { "ID", "Subject", "Message", "Sent Date", "Read" };
                        foreach (var x in await _context.Notifications.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.NotificationId,
                                ["Subject"] = x.Subject ?? "",
                                ["Message"] = x.Message ?? "",
                                ["Sent Date"] = x.SentDate.ToString(),
                                ["Read"] = x.IsRead == true ? "Yes" : "No"
                            });
                        }
                        break;

                    case "NOTIFICATIONTEMPLATE":
                        columns = new List<string> { "ID", "Template Name", "Code", "Subject", "Status" };
                        foreach (var x in await _context.NotificationTemplates.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.TemplateId,
                                ["Template Name"] = x.TemplateName ?? "",
                                ["Code"] = x.TemplateCode ?? "",
                                ["Subject"] = x.Subject ?? "",
                                ["Status"] = x.IsActive == true ? "Active" : "Inactive"
                            });
                        }
                        break;

                    case "AUDITLOG":
                        columns = new List<string> { "ID", "Action", "Module", "Record ID", "Date" };
                        foreach (var x in await _context.AuditLogs.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.AuditId,
                                ["Action"] = x.Action ?? "",
                                ["Module"] = x.RecordType ?? "",
                                ["Record ID"] = x.RecordId,
                                ["Date"] = x.Timestamp.ToString()
                            });
                        }
                        break;

                    case "BACKUP":
                        columns = new List<string> { "ID", "Date", "Type", "File Size (MB)", "Status" };
                        foreach (var x in await _context.DataBackupLogs.ToListAsync())
                        {
                            rows.Add(new Dictionary<string, object>
                            {
                                ["ID"] = x.BackupId,
                                ["Date"] = x.BackupDate.ToString(),
                                ["Type"] = x.BackupType ?? "",
                                ["File Size (MB)"] = x.FileSizeMb.ToString("N2"),
                                ["Status"] = x.Status ?? ""
                            });
                        }
                        break;

                    
                    // ============ PAYROLL REPORTS ============
                    case "PAYROLLREGISTER":
                        columns = new List<string> { "Code", "Employee", "Basic", "Gross", "OT", "Bonus", "Earnings", "Deductions", "NetPay" };
                        {
                            var runId = await _context.PayrollRuns.Where(r => r.IsDeleted != true).OrderByDescending(r => r.RunId).Select(r => r.RunId).FirstOrDefaultAsync();
                            foreach (var x in await _context.PayrollRegisters.Include(r => r.Employee).Where(r => r.RunId == runId).OrderBy(r => r.Employee.EmployeeCode).ToListAsync())
                            {
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["Code"] = x.Employee.EmployeeCode ?? "",
                                    ["Employee"] = (x.Employee.FirstName ?? "") + " " + (x.Employee.LastName ?? ""),
                                    ["Basic"] = x.BasicSalary.ToString("N2"),
                                    ["Gross"] = x.GrossSalary.ToString("N2"),
                                    ["OT"] = x.OvertimeAmount.ToString("N2"),
                                    ["Bonus"] = x.BonusAmount.ToString("N2"),
                                    ["Earnings"] = x.TotalEarnings.ToString("N2"),
                                    ["Deductions"] = x.TotalDeductions.ToString("N2"),
                                    ["NetPay"] = x.NetPay.ToString("N2")
                                });
                            }
                        }
                        break;

                    case "ATTENDANCESUMMARY":
                        columns = new List<string> { "Code", "Employee", "Present", "Absent", "Late", "OT Hours" };
                        {
                            var y = DateTime.Now.Year;
                            var m = DateTime.Now.Month;
                            var start = new DateOnly(y, m, 1);
                            var end = new DateOnly(y, m, DateTime.DaysInMonth(y, m));
                            foreach (var x in await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true).ToListAsync())
                            {
                                var att = await _context.EmployeeAttendances.Where(a => a.EmployeeId == x.EmployeeId && a.AttendanceDate >= start && a.AttendanceDate <= end).ToListAsync();
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["Code"] = x.EmployeeCode ?? "",
                                    ["Employee"] = (x.FirstName ?? "") + " " + (x.LastName ?? ""),
                                    ["Present"] = att.Count(a => a.Status != "Absent").ToString(),
                                    ["Absent"] = att.Count(a => a.Status == "Absent").ToString(),
                                    ["Late"] = att.Count(a => (a.LateMinutes ?? 0) > 0).ToString(),
                                    ["OT Hours"] = att.Sum(a => a.OTHours ?? 0).ToString("N2")
                                });
                            }
                        }
                        break;

                    case "LEAVEREPORT":
                        columns = new List<string> { "Code", "Employee", "LeaveType", "From", "To", "Days", "Status" };
                        {
                            foreach (var x in await _context.LeaveRequests.Include(l => l.Employee).Include(l => l.LeaveType).Where(l => l.IsDeleted != true).OrderByDescending(l => l.FromDate).ToListAsync())
                            {
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["Code"] = x.Employee?.EmployeeCode ?? "",
                                    ["Employee"] = (x.Employee?.FirstName ?? "") + " " + (x.Employee?.LastName ?? ""),
                                    ["LeaveType"] = x.LeaveType?.LeaveName ?? "-",
                                    ["From"] = x.FromDate.ToString("dd-MMM-yyyy"),
                                    ["To"] = x.ToDate.ToString("dd-MMM-yyyy"),
                                    ["Days"] = x.TotalDays.ToString(),
                                    ["Status"] = x.Status ?? ""
                                });
                            }
                        }
                        break;

                    case "LOANREPORT":
                        columns = new List<string> { "LoanNo", "Code", "Employee", "Amount", "Monthly", "Recovered", "Remaining", "Status" };
                        {
                            foreach (var x in await _context.Loans.Include(l => l.Employee).Where(l => l.IsDeleted != true).OrderByDescending(l => l.LoanId).ToListAsync())
                            {
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["LoanNo"] = x.LoanNo ?? "",
                                    ["Code"] = x.Employee?.EmployeeCode ?? "",
                                    ["Employee"] = (x.Employee?.FirstName ?? "") + " " + (x.Employee?.LastName ?? ""),
                                    ["Amount"] = x.LoanAmount.ToString("N2"),
                                    ["Monthly"] = x.MonthlyRecovery.ToString("N2"),
                                    ["Recovered"] = x.TotalRecovered.ToString("N2"),
                                    ["Remaining"] = x.RemainingAmount.ToString("N2"),
                                    ["Status"] = x.Status ?? ""
                                });
                            }
                        }
                        break;

                    case "OVERTIMEREPORT":
                        columns = new List<string> { "Date", "Code", "Employee", "Hours", "Rate", "Multiplier", "Amount", "Status" };
                        {
                            foreach (var x in await _context.Overtimes.Include(o => o.Employee).Where(o => o.IsDeleted != true).OrderByDescending(o => o.OvertimeDate).ToListAsync())
                            {
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["Date"] = x.OvertimeDate.ToString("dd-MMM-yyyy"),
                                    ["Code"] = x.Employee?.EmployeeCode ?? "",
                                    ["Employee"] = (x.Employee?.FirstName ?? "") + " " + (x.Employee?.LastName ?? ""),
                                    ["Hours"] = x.OTHours.ToString("N2"),
                                    ["Rate"] = x.OTRate.ToString("N2"),
                                    ["Multiplier"] = x.OTMultiplier.ToString("N2"),
                                    ["Amount"] = x.OTAmount.ToString("N2"),
                                    ["Status"] = x.Status ?? ""
                                });
                            }
                        }
                        break;

                    case "SALARYREPORT":
                        columns = new List<string> { "Code", "Employee", "Effective", "Basic", "Allowances", "Deductions", "NetSalary" };
                        {
                            foreach (var x in await _context.EmployeeSalaries1.Include(s => s.Employee).Where(s => s.IsDeleted != true).OrderBy(s => s.Employee.EmployeeCode).ToListAsync())
                            {
                                rows.Add(new Dictionary<string, object>
                                {
                                    ["Code"] = x.Employee?.EmployeeCode ?? "",
                                    ["Employee"] = (x.Employee?.FirstName ?? "") + " " + (x.Employee?.LastName ?? ""),
                                    ["Effective"] = x.EffectiveDate.ToString("dd-MMM-yyyy"),
                                    ["Basic"] = x.BasicSalary.ToString("N2"),
                                    ["Allowances"] = (x.TotalAllowances ?? 0).ToString("N2"),
                                    ["Deductions"] = (x.TotalDeductions ?? 0).ToString("N2"),
                                    ["NetSalary"] = (x.NetSalary ?? 0).ToString("N2")
                                });
                            }
                        }
                        break;
default:
                        columns = new List<string> { "ID", "Reference", "Description", "Date" };
                        break;
                }
            }
            catch (Exception ex)
            {
                columns = new List<string> { "Error" };
                rows.Add(new Dictionary<string, object> { ["Error"] = ex.Message });
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
        public IActionResult ExportExcel(string code) => Content($"Excel Export - {code}");

        [HttpGet]
        public IActionResult ExportCsv(string code) => Content($"CSV Export - {code}");

                private static string GetReportName(string code) => code?.ToUpper() switch
        {
            "COMPANY" => "Companies",
            "BRANCH" => "Branches",
            "DEPARTMENT" => "Departments",
            "DESIGNATION" => "Designations",
            "WAREHOUSE" => "Warehouses",
            "ZONE" => "Zones",
            "RACK" => "Racks",
            "BIN" => "Bins",
            "CUSTOMER" => "Customers",
            "SUPPLIER" => "Suppliers",
            "CUSTOMERCOMPLAINT" => "Customer Complaints",
            "CUSTOMERFEEDBACK" => "Customer Feedback",
            "CUSTOMERINQUIRY" => "Customer Inquiries",
            "CATEGORY" => "Categories",
            "SUBCATEGORY" => "Sub Categories",
            "BRAND" => "Brands",
            "UNIT" => "Units",
            "PRODUCT" => "Products",
            "PURCHASEMASTER" => "Purchase Orders",
            "GRN" => "Goods Receipt",
            "PURCHASERETURN" => "Purchase Returns",
            "SALES" => "Sales Orders",
            "INVOICE" => "Invoices",
            "DISPATCH" => "Dispatch",
            "PICKINGLIST" => "Picking Lists",
            "PICKING" => "Picking",
            "SALESRETURN" => "Sales Returns",
            "STOCKLEDGER" => "Stock Ledger",
            "STOCKADJUSTMENT" => "Stock Adjustments",
            "STOCKTRANSFER" => "Stock Transfers",
            "CYCLECOUNT" => "Cycle Count",
            "ADJUSTMENT" => "Adjustments",
            "BATCHTRACKING" => "Batch Tracking",
            "SERIALNUMBER" => "Serial Numbers",
            "TRANSFER" => "Transfers",
            "CHARTOFACCOUNT" => "Chart of Accounts",
            "JOURNALVOUCHER" => "Journal Vouchers",
            "PAYMENTVOUCHER" => "Payment Vouchers",
            "RECEIPTVOUCHER" => "Receipt Vouchers",
            "BANKACCOUNT" => "Bank Accounts",
            "BANKRECONCILIATION" => "Bank Reconciliation",
            "TAXSETTING" => "Tax Settings",
            "EMPLOYEE" => "Employees",
            "ATTENDANCE" => "Attendance",
            "LEAVE" => "Leaves",
            "PAYROLL" => "Payroll",
            "EMPLOYEEBANKACCOUNT" => "Employee Bank Accounts",
            "EMPLOYEESALARY" => "Employee Salaries",
            "SHIFT" => "Shifts",
            "PAYROLLREGISTER" => "Payroll Register",
            "ATTENDANCESUMMARY" => "Attendance Summary",
            "LEAVEREPORT" => "Leave Report",
            "LOANREPORT" => "Loan Report",
            "OVERTIMEREPORT" => "Overtime Report",
            "SALARYREPORT" => "Salary Report",
            "WORKFLOWDASHBOARD" => "Workflow Dashboard",
            "WORKFLOWDEFINITION" => "Workflow Definitions",
            "WORKFLOWSTAGE" => "Workflow Stages",
            "WORKFLOWTRANSITION" => "Workflow Transitions",
            "WORKFLOWCONDITION" => "Workflow Conditions",
            "WORKFLOWASSIGNMENT" => "Workflow Assignments",
            "WORKFLOWDEADLINE" => "Workflow Deadlines",
            "WORKFLOWINSTANCE" => "Workflow Instances",
            "WORKFLOWINSTANCELOG" => "Workflow Instance Logs",
            "SYSTEMADMINDASHBOARD" => "Admin Dashboard",
            "USER" => "Users",
            "USERPROFILE" => "User Profile",
            "ROLE" => "Roles",
            "SYSTEMSETTING" => "System Settings",
            "NOTIFICATION" => "Notifications",
            "NOTIFICATIONTEMPLATE" => "Notification Templates",
            "BACKUP" => "Database Backups",
            _ => code ?? "Report"
        };
    }
}