using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly AppDbContext _context;
        public LoanController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Loans
                .Include(x => x.Employee)
                .Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.LoanId)
                .Select(x => new {
                    loanId = x.LoanId, loanNo = x.LoanNo,
                    employeeName = x.Employee.FirstName + " " + x.Employee.LastName,
                    employeeCode = x.Employee.EmployeeCode,
                    loanAmount = x.LoanAmount,
                    monthlyRecovery = x.MonthlyRecovery,
                    totalRecovered = x.TotalRecovered,
                    remainingAmount = x.RemainingAmount,
                    paidInstallments = x.PaidInstallments,
                    totalInstallments = x.TotalInstallments,
                    status = x.Status,
                    issuedDate = x.IssuedDate.ToString("yyyy-MM-dd")
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var l = await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { employeeId = e.EmployeeId, employeeCode = e.EmployeeCode, fullName = e.FirstName + " " + e.LastName })
                .OrderBy(e => e.fullName).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int loanId, int employeeId, string loanNo, decimal loanAmount,
            decimal monthlyRecovery, int totalInstallments, string issuedDate, string? reason, string status)
        {
            try
            {
                if (employeeId <= 0 || loanAmount <= 0) return Json(new { success = false, message = "Employee and Amount required." });
                if (!DateOnly.TryParse(issuedDate, out var dt)) return Json(new { success = false, message = "Invalid date." });

                Loan e;
                if (loanId > 0)
                {
                    e = await _context.Loans.FindAsync(loanId);
                    if (e == null) return Json(new { success = false, message = "Not found" });
                }
                else
                {
                    e = new Loan {
                        LoanNo = string.IsNullOrWhiteSpace(loanNo) ? "LN-" + DateTime.Now.ToString("yyyyMMddHHmmss") : loanNo,
                        RemainingAmount = loanAmount,
                        TotalRecovered = 0,
                        PaidInstallments = 0,
                        IsDeleted = false, CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                    };
                    _context.Loans.Add(e);
                }

                e.EmployeeId = employeeId;
                e.LoanAmount = loanAmount;
                e.MonthlyRecovery = monthlyRecovery;
                e.TotalInstallments = totalInstallments;
                e.IssuedDate = dt;
                e.Reason = reason;
                e.Status = status ?? "Pending";

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = loanId > 0 ? "Updated!" : "Created!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var l = await _context.Loans.FindAsync(id);
            if (l == null) return Json(new { success = false, message = "Not found" });
            l.Status = "Active";
            l.StartRecoveryDate = DateOnly.FromDateTime(DateTime.Today);
            l.ApprovedBy = Uid().ToString();
            l.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Loan approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var l = await _context.Loans.FindAsync(id);
            if (l == null) return Json(new { success = false, message = "Not found" });
            l.IsDeleted = true; l.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
