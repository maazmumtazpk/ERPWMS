using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        public IActionResult Index() => View();

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.Departments.Where(d => d.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(d => d.IsActive == true),
                inactive = l.Count(d => d.IsActive != true),
                addedThisMonth = l.Count(d =>
                    d.CreatedDate.HasValue &&
                    d.CreatedDate.Value.Month == DateTime.Now.Month &&
                    d.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Departments
                .Include(d => d.Branch)
                .ThenInclude(b => b.Company)
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.DepartmentId)
                .Select(d => new
                {
                    departmentId = d.DepartmentId,
                    departmentName = d.DepartmentName,
                    departmentCode = d.DepartmentCode ?? "",
                    branchName = d.Branch != null ? d.Branch.BranchName : "N/A",
                    companyName = d.Branch != null && d.Branch.Company != null ? d.Branch.Company.CompanyName : "N/A",
                    isActive = d.IsActive ?? false,
                    createdDate = d.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET BRANCHES
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var l = await _context.Branches
                .Where(b => b.IsActive == true && b.IsDeleted != true)
                .Select(b => new { branchId = b.BranchId, branchName = b.BranchName })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddDepartmentPartial()
        {
            return PartialView("_AddDepartmentForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditDepartmentPartial(int id)
        {
            var d = await _context.Departments
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.DepartmentId == id);
            if (d == null) return NotFound();

            ViewBag.BranchName = d.Branch != null ? d.Branch.BranchName : "N/A";
            return PartialView("_EditDepartmentForm", d);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string departmentName, string? departmentCode, int branchId, bool isActive = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(departmentName))
                    return Json(new { success = false, message = "Department name required." });
                if (branchId <= 0)
                    return Json(new { success = false, message = "Please select a branch." });

                var b = await _context.Branches.FindAsync(branchId);
                if (b == null)
                    return Json(new { success = false, message = "Branch not found." });

                var d = await _context.Departments.AnyAsync(x =>
                    x.DepartmentName == departmentName &&
                    x.BranchId == branchId &&
                    x.IsDeleted != true);
                if (d)
                    return Json(new { success = false, message = "Department already exists in this branch." });

                _context.Departments.Add(new Department
                {
                    DepartmentName = departmentName.Trim(),
                    DepartmentCode = string.IsNullOrWhiteSpace(departmentCode) ? null : departmentCode.Trim(),
                    BranchId = branchId,
                    CompanyId = b.CompanyId,
                    IsActive = isActive,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Department registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int departmentId, string departmentName, string? departmentCode, bool isActive = true)
        {
            try
            {
                var d = await _context.Departments.FindAsync(departmentId);
                if (d == null)
                    return Json(new { success = false, message = "Not found" });

                d.DepartmentName = departmentName.Trim();
                d.DepartmentCode = string.IsNullOrWhiteSpace(departmentCode) ? null : departmentCode.Trim();
                d.IsActive = isActive;
                d.UpdatedBy = Uid().ToString();
                d.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Department updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var d = await _context.Departments.FindAsync(id);
                if (d == null)
                    return Json(new { success = false, message = "Not found" });

                d.IsDeleted = true;
                d.IsActive = false;
                d.DeletedBy = Uid().ToString();
                d.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Department deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // TOGGLE ACTIVE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var d = await _context.Departments.FindAsync(id);
                if (d == null)
                    return Json(new { success = false, message = "Not found" });

                d.IsActive = !(d.IsActive ?? false);
                d.UpdatedBy = Uid().ToString();
                d.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = d.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // PROFILE
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var d = await _context.Departments
                .Include(x => x.Branch)
                .ThenInclude(b => b.Company)
                .FirstOrDefaultAsync(x => x.DepartmentId == id);
            if (d == null) return NotFound();

            ViewBag.BranchName = d.Branch != null ? d.Branch.BranchName : "N/A";
            ViewBag.CompanyName = d.Branch != null && d.Branch.Company != null ? d.Branch.Company.CompanyName : "N/A";
            ViewBag.DesignationCount = await _context.Designations.CountAsync(x => x.DepartmentId == id);
            ViewBag.EmployeeCount = await _context.Employees.CountAsync(x => x.DepartmentId == id);

            return View(d);
        }
    }
}
