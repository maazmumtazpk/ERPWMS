using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class BinController : Controller
    {
        private readonly AppDbContext _context;
        public BinController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet] public IActionResult Index() => View();

        // --- STATISTICS ---
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var list = await _context.Bins.Where(b => b.IsDeleted != true).ToListAsync();
            var now = DateTime.Now;
            return Json(new {
                total = list.Count,
                active = list.Count(b => b.IsActive == true),
                inactive = list.Count(b => b.IsActive != true),
                addedThisMonth = list.Count(b => b.CreatedDate.HasValue && b.CreatedDate.Value.Month == now.Month && b.CreatedDate.Value.Year == now.Year)
            });
        }

        // --- GET ALL — with Rack ? Zone ? Warehouse joins ---
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await (from b in _context.Bins
                                  join r in _context.Racks on b.RackId equals r.RackId into rj
                                  from r in rj.DefaultIfEmpty()
                                  join z in _context.Zones on r.ZoneId equals z.ZoneId into zj
                                  from z in zj.DefaultIfEmpty()
                                  join w in _context.Warehouses on z.WarehouseId equals w.WarehouseId into wj
                                  from w in wj.DefaultIfEmpty()
                                  where b.IsDeleted != true
                                  orderby b.BinId
                                  select new {
                                      binId = b.BinId,
                                      binName = b.BinName,
                                      binCode = b.BinCode,
                                      rackId = b.RackId,
                                      rackName = r != null ? r.RackName : "-",
                                      zoneName = z != null ? z.ZoneName : "-",
                                      warehouseName = w != null ? w.WarehouseName : "-",
                                      isActive = b.IsActive == true
                                  }).ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // --- RACKS DROPDOWN ---
        [HttpGet]
        public async Task<IActionResult> GetRacksDropdown()
        {
            try
            {
                var list = await _context.Racks
                    .Where(r => r.IsDeleted != true)
                    .OrderBy(r => r.RackName)
                    .Select(r => new { rackId = r.RackId, rackName = r.RackName })
                    .ToListAsync();
                return Json(list);
            }
            catch (Exception ex) { return Json(new { error = ex.Message }); }
        }

        // --- PARTIALS ---
        [HttpGet] public IActionResult AddBinPartial() => PartialView("_AddBinForm");

        [HttpGet]
        public async Task<IActionResult> EditBinPartial(int id)
        {
            var b = await _context.Bins.FindAsync(id);
            if (b == null) return NotFound();
            return PartialView("_EditBinForm", b);
        }

        // --- REGISTER ---
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int rackId, string binName, string binCode, bool isActive = true)
        {
            try
            {
                if (rackId <= 0) return Json(new { success = false, message = "Please select rack" });
                if (string.IsNullOrWhiteSpace(binName)) return Json(new { success = false, message = "Bin name required" });

                // UNIQUE: BinName per rack
                var dup = await _context.Bins.AnyAsync(b => b.RackId == rackId && b.BinName == binName.Trim() && b.IsDeleted != true);
                if (dup) return Json(new { success = false, message = "Bin with this name already exists in selected rack" });

                _context.Bins.Add(new Bin {
                    RackId = rackId,
                    BinName = binName.Trim(),
                    BinCode = binCode ?? "",
                    IsActive = isActive,
                    IsDeleted = false,
                    IsOccupied = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // --- EDIT ---
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int binId, int rackId, string binName, string binCode, bool isActive = true)
        {
            try
            {
                var b = await _context.Bins.FindAsync(binId);
                if (b == null) return Json(new { success = false, message = "Not found" });

                var dup = await _context.Bins.AnyAsync(x => x.BinId != binId && x.RackId == rackId && x.BinName == binName.Trim() && x.IsDeleted != true);
                if (dup) return Json(new { success = false, message = "Bin with this name already exists in selected rack" });

                b.RackId = rackId;
                b.BinName = binName.Trim();
                b.BinCode = binCode ?? "";
                b.IsActive = isActive;
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // --- DELETE ---
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var b = await _context.Bins.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Not found" });
                b.IsDeleted = true;
                b.IsActive = false;
                b.DeletedBy = Uid().ToString();
                b.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // --- TOGGLE ---
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var b = await _context.Bins.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Not found" });
                b.IsActive = !(b.IsActive ?? false);
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = b.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
