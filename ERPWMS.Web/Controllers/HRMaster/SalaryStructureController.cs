using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Infrastructure.Services;
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
    public class SalaryStructureController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISalaryStructureService _svc;

        public SalaryStructureController(AppDbContext c, ISalaryStructureService svc) { _context = c; _svc = svc; }

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
            var l = await _context.SalaryStructures
                .Where(s => s.IsDeleted != true)
                .OrderBy(s => s.StructureName)
                .Select(s => new {
                    structureId = s.StructureId,
                    structureCode = s.StructureCode,
                    structureName = s.StructureName,
                    grade = s.Grade,
                    minSalary = s.MinSalary,
                    maxSalary = s.MaxSalary,
                    isActive = s.IsActive,
                    componentCount = _context.SalaryStructureComponents.Count(c => c.StructureId == s.StructureId && c.IsDeleted != true)
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _context.SalaryStructures.FindAsync(id);
            if (s == null) return NotFound();
            return Json(new {
                structureId = s.StructureId,
                structureCode = s.StructureCode,
                structureName = s.StructureName,
                grade = s.Grade,
                description = s.Description,
                minSalary = s.MinSalary,
                maxSalary = s.MaxSalary,
                isActive = s.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int structureId, string structureCode, string structureName,
            string grade, string? description, decimal? minSalary, decimal? maxSalary, bool isActive)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(structureName)) return Json(new { success = false, message = "Name required." });
                if (string.IsNullOrWhiteSpace(structureCode)) return Json(new { success = false, message = "Code required." });

                SalaryStructure e;
                if (structureId > 0)
                {
                    e = await _context.SalaryStructures.FindAsync(structureId);
                    if (e == null) return Json(new { success = false, message = "Not found" });
                    e.StructureCode = structureCode;
                    e.StructureName = structureName;
                    e.Grade = grade ?? "Worker";
                    e.Description = description;
                    e.MinSalary = minSalary;
                    e.MaxSalary = maxSalary;
                    e.IsActive = isActive;
                    e.UpdatedBy = Uid().ToString();
                    e.UpdatedDate = DateTime.Now;
                }
                else
                {
                    var exists = await _context.SalaryStructures.AnyAsync(x => x.StructureCode == structureCode && x.IsDeleted != true);
                    if (exists) return Json(new { success = false, message = "Code already exists." });
                    e = new SalaryStructure {
                        StructureCode = structureCode, StructureName = structureName,
                        Grade = grade ?? "Worker", Description = description,
                        MinSalary = minSalary, MaxSalary = maxSalary, IsActive = isActive,
                        IsDeleted = false, CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                    };
                    _context.SalaryStructures.Add(e);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = structureId > 0 ? "Updated!" : "Created!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _context.SalaryStructures.FindAsync(id);
            if (e == null) return Json(new { success = false, message = "Not found" });
            e.IsDeleted = true; e.IsActive = false;
            e.UpdatedBy = Uid().ToString(); e.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
