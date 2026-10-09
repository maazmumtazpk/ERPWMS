using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        public CompanyController(AppDbContext context) { _context = context; }

        public IActionResult Index() { return View(); }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var q = _context.Companies.Where(c => c.IsDeleted != true);
            var total = await q.CountAsync();
            var active = await q.CountAsync(c => c.IsActive == true);
            var inactive = await q.CountAsync(c => c.IsActive != true);
            var now = DateTime.Now;
            var month = await q.CountAsync(c => c.CreatedDate.HasValue && c.CreatedDate.Value.Month == now.Month && c.CreatedDate.Value.Year == now.Year);
            return Json(new { total, active, inactive, addedThisMonth = month });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var list = await _context.Companies
                .Where(c => c.IsDeleted != true)
                .OrderBy(c => c.CompanyId)
                .Select(c => new {
                    companyId = c.CompanyId,
                    companyName = c.CompanyName,
                    registrationNo = c.RegistrationNo,
                    email = c.Email,
                    phone = c.Phone,
                    isActive = c.IsActive == true,
                    createdDate = c.CreatedDate,
                    branchCount = c.Branches.Count()
                }).ToListAsync();
            return Json(list);
        }

        [HttpGet]
        public IActionResult AddCompanyPartial() { return PartialView("_AddCompanyForm"); }

        [HttpGet]
        public async Task<IActionResult> EditCompanyPartial(int id)
        {
            var c = await _context.Companies.FindAsync(id);
            if (c == null) return NotFound();
            return PartialView("_EditCompanyForm", c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Company model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName))
                return Json(new { success = false, message = "Company name is required" });

            if (!string.IsNullOrWhiteSpace(model.RegistrationNo))
            {
                var dup = await _context.Companies
                    .AnyAsync(c => c.IsDeleted != true && c.RegistrationNo == model.RegistrationNo);
                if (dup)
                    return Json(new { success = false, message = "Registration No. \"" + model.RegistrationNo + "\" already exists. Please use a different one." });
            }

            var user = User.Identity?.Name ?? "System";
            int uid = 1;
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (idClaim != null && int.TryParse(idClaim, out var pid)) uid = pid;

            model.CompanyId = 0;
            model.IsActive = true;
            model.IsDeleted = false;
            model.CreatedBy = uid.ToString();
            model.CreatedByName = user;
            model.CreatedDate = DateTime.Now;

            try
            {
                _context.Companies.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Company registered successfully" });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Registration No. already exists. Please use a different one." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName))
                return Json(new { success = false, message = "Company name is required" });

            var c = await _context.Companies.FindAsync(model.CompanyId);
            if (c == null) return Json(new { success = false, message = "Company not found" });

            if (!string.IsNullOrWhiteSpace(model.RegistrationNo))
            {
                var dup = await _context.Companies
                    .AnyAsync(x => x.IsDeleted != true && x.CompanyId != model.CompanyId && x.RegistrationNo == model.RegistrationNo);
                if (dup)
                    return Json(new { success = false, message = "Registration No. \"" + model.RegistrationNo + "\" already exists." });
            }

            c.CompanyName = model.CompanyName;
            c.RegistrationNo = model.RegistrationNo;
            c.Address = model.Address;
            c.Phone = model.Phone;
            c.Email = model.Email;
            c.LogoUrl = model.LogoUrl;
            c.UpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Company updated successfully" });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Update failed. Duplicate registration number." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Companies.FindAsync(id);
            if (c == null) return Json(new { success = false });
            c.IsDeleted = true;
            c.IsActive = false;
            c.DeletedByName = User.Identity?.Name ?? "System";
            c.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Company deleted" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var c = await _context.Companies.FindAsync(id);
            if (c == null) return Json(new { success = false });
            c.IsActive = !(c.IsActive ?? false);
            c.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = c.IsActive == true ? "Company activated" : "Company deactivated" });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var c = await _context.Companies
                .Include(x => x.Branches)
                .Include(x => x.Departments)
                .FirstOrDefaultAsync(x => x.CompanyId == id);
            if (c == null) return NotFound();
            ViewBag.BranchCount = c.Branches.Count();
            ViewBag.DepartmentCount = c.Departments.Count();
            return View(c);
        }
    }
}
