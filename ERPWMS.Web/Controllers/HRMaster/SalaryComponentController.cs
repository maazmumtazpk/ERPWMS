using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class SalaryComponentController : Controller
    {
        private readonly AppDbContext _context;
        public SalaryComponentController(AppDbContext c) { _context = c; }

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
            var l = await _context.SalaryComponents
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.DisplayOrder).ThenBy(x => x.ComponentName)
                .Select(x => new {
                    componentId = x.ComponentId,
                    componentCode = x.ComponentCode,
                    componentName = x.ComponentName,
                    componentType = x.ComponentType,
                    calculationType = x.CalculationType,
                    amount = x.Amount,
                    isTaxable = x.IsTaxable,
                    isEarning = x.IsEarning,
                    isActive = x.IsActive
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _context.SalaryComponents.FindAsync(id);
            if (x == null) return NotFound();
            return Json(new {
                componentId = x.ComponentId,
                componentCode = x.ComponentCode,
                componentName = x.ComponentName,
                componentType = x.ComponentType,
                calculationType = x.CalculationType,
                amount = x.Amount,
                percentage = x.Percentage,
                isTaxable = x.IsTaxable,
                isEarning = x.IsEarning,
                displayOrder = x.DisplayOrder,
                showInPayslip = x.ShowInPayslip,
                description = x.Description,
                isActive = x.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int componentId, string componentCode, string componentName,
            string componentType, string calculationType, decimal? amount, decimal? percentage,
            bool isTaxable, bool isEarning, int displayOrder, bool showInPayslip, string? description, bool isActive)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(componentName)) return Json(new { success = false, message = "Name required." });
                if (string.IsNullOrWhiteSpace(componentCode)) return Json(new { success = false, message = "Code required." });

                SalaryComponent e;
                if (componentId > 0)
                {
                    e = await _context.SalaryComponents.FindAsync(componentId);
                    if (e == null) return Json(new { success = false, message = "Not found" });
                }
                else
                {
                    var exists = await _context.SalaryComponents.AnyAsync(x => x.ComponentCode == componentCode && x.IsDeleted != true);
                    if (exists) return Json(new { success = false, message = "Code already exists." });
                    e = new SalaryComponent { IsDeleted = false, CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now };
                    _context.SalaryComponents.Add(e);
                }

                e.ComponentCode = componentCode;
                e.ComponentName = componentName;
                e.ComponentType = componentType ?? "Earning";
                e.CalculationType = calculationType ?? "Fixed";
                e.Amount = amount;
                e.Percentage = percentage;
                e.IsTaxable = isTaxable;
                e.IsEarning = isEarning;
                e.DisplayOrder = displayOrder;
                e.ShowInPayslip = showInPayslip;
                e.Description = description;
                e.IsActive = isActive;
                if (componentId > 0) { e.UpdatedBy = Uid().ToString(); e.UpdatedDate = DateTime.Now; }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = componentId > 0 ? "Updated!" : "Created!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _context.SalaryComponents.FindAsync(id);
            if (e == null) return Json(new { success = false, message = "Not found" });
            e.IsDeleted = true; e.IsActive = false;
            e.UpdatedBy = Uid().ToString(); e.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
