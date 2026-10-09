using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Finance
{
    [Authorize]
    public class TaxSettingController : Controller
    {
        private readonly AppDbContext _context;

        public TaxSettingController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.TaxSettings.Where(t => t.IsDeleted == false).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(t => t.IsActive == true),
                inactive = l.Count(t => t.IsActive != true),
                addedThisMonth = l.Count(t =>
                    t.CreatedDate.HasValue &&
                    t.CreatedDate.Value.Month == DateTime.Now.Month &&
                    t.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.TaxSettings
                .Where(t => t.IsDeleted == false)
                .OrderBy(t => t.TaxId)
                .Select(t => new
                {
                    taxId = t.TaxId,
                    taxName = t.TaxName,
                    taxCode = t.TaxCode,
                    taxRate = t.TaxRate,
                    isActive = t.IsActive ?? false,
                    isDefault = t.IsDefault ?? false,
                    effectiveFrom = t.EffectiveFrom,
                    createdDate = t.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddTaxPartial() => PartialView("_AddTaxForm");

        [HttpGet]
        public async Task<IActionResult> EditTaxPartial(int id)
        {
            var t = await _context.TaxSettings.FindAsync(id);
            if (t == null) return NotFound();
            return PartialView("_EditTaxForm", t);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string taxName,
            string? taxCode,
            decimal taxRate,
            DateOnly effectiveFrom,
            DateOnly? effectiveTo,
            bool isDefault = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(taxName))
                    return Json(new { success = false, message = "Tax name required." });

                _context.TaxSettings.Add(new TaxSetting
                {
                    TaxName = taxName.Trim(),
                    TaxCode = taxCode ?? "",
                    TaxRate = taxRate,
                    CompanyId = 1,
                    IsActive = true,
                    IsDefault = isDefault,
                    EffectiveFrom = effectiveFrom,
                    EffectiveTo = effectiveTo,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Tax setting created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int taxId,
            string taxName,
            string? taxCode,
            decimal taxRate,
            bool isDefault = false,
            bool isActive = true)
        {
            try
            {
                var t = await _context.TaxSettings.FindAsync(taxId);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                t.TaxName = taxName.Trim();
                t.TaxCode = taxCode;
                t.TaxRate = taxRate;
                t.IsDefault = isDefault;
                t.IsActive = isActive;
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Tax updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var t = await _context.TaxSettings.FindAsync(id);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                t.IsDeleted = true;
                t.IsActive = false;
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE ACTIVE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var t = await _context.TaxSettings.FindAsync(id);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                t.IsActive = !(t.IsActive ?? false);
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = t.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}