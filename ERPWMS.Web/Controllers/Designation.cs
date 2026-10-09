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
    public class DesignationController : Controller
    {
        private readonly AppDbContext _context;

        public DesignationController(AppDbContext context)
        {
            _context = context;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

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
            var l = await _context.Designations.Where(d => d.IsDeleted != true).ToListAsync();
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
            var l = await _context.Designations
                .Include(d => d.Department)
                .ThenInclude(dep => dep.Company)
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.DesignationId)
                .Select(d => new
                {
                    designationId = d.DesignationId,
                    designationName = d.DesignationName,
                    rankLevel = d.RankLevel,                // ✅ FIX: int? direct
                    departmentName = d.Department != null ? d.Department.DepartmentName : "N/A",
                    companyName = d.Department != null && d.Department.Company != null ? d.Department.Company.CompanyName : "N/A",
                    isActive = d.IsActive ?? false,
                    createdDate = d.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET DEPARTMENTS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var l = await _context.Departments
                .Where(d => d.IsActive == true && d.IsDeleted != true)
                .Select(d => new { departmentId = d.DepartmentId, departmentName = d.DepartmentName })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddDesignationPartial()
        {
            return PartialView("_AddDesignationForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditDesignationPartial(int id)
        {
            var d = await _context.Designations
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.DesignationId == id);
            if (d == null) return NotFound();

            ViewBag.DepartmentName = d.Department != null ? d.Department.DepartmentName : "N/A";
            return PartialView("_EditDesignationForm", d);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string designationName, int rankLevel, int departmentId, bool isActive = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(designationName))
                    return Json(new { success = false, message = "Designation name required." });
                if (departmentId <= 0)
                    return Json(new { success = false, message = "Please select a department." });

                var d = await _context.Designations.AnyAsync(x =>
                    x.DesignationName == designationName &&
                    x.DepartmentId == departmentId &&
                    x.IsDeleted != true);
                if (d)
                    return Json(new { success = false, message = "Designation already exists in this department." });

                _context.Designations.Add(new Designation
                {
                    DesignationName = designationName.Trim(),
                    RankLevel = rankLevel > 0 ? rankLevel : (int?)null,
                    DepartmentId = departmentId,
                    IsActive = isActive,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Designation registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int designationId, string designationName, int rankLevel, bool isActive = true)
        {
            try
            {
                var d = await _context.Designations.FindAsync(designationId);
                if (d == null)
                    return Json(new { success = false, message = "Not found" });

                d.DesignationName = designationName.Trim();
                d.RankLevel = rankLevel > 0 ? rankLevel : (int?)null;
                d.IsActive = isActive;
                d.UpdatedBy = Uid().ToString();
                d.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Designation updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var d = await _context.Designations.FindAsync(id);
                if (d == null)
                    return Json(new { success = false, message = "Not found" });

                d.IsDeleted = true;
                d.IsActive = false;
                d.DeletedBy = Uid().ToString();
                d.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Designation deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // TOGGLE ACTIVE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var d = await _context.Designations.FindAsync(id);
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
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // PROFILE
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var d = await _context.Designations
                .Include(x => x.Department)
                .ThenInclude(dep => dep.Company)
                .FirstOrDefaultAsync(x => x.DesignationId == id);
            if (d == null) return NotFound();

            ViewBag.DepartmentName = d.Department != null ? d.Department.DepartmentName : "N/A";
            ViewBag.CompanyName = d.Department != null && d.Department.Company != null ? d.Department.Company.CompanyName : "N/A";
            ViewBag.EmployeeCount = await _context.Employees.CountAsync(x => x.DesignationId == id);

            return View(d);
        }
    }
}