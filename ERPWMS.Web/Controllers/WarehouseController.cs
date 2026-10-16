using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Web.ViewModels.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly AppDbContext _context;

        public WarehouseController(AppDbContext context)
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
        // BASIC VIEWS
        // ============================================================
        [HttpGet] public IActionResult Index() => View();
        [HttpGet] public IActionResult Zones() => View();
        [HttpGet] public IActionResult Racks() => View();
        [HttpGet] public IActionResult Bins() => View();

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.Warehouses.Where(w => w.IsDeleted != true).ToListAsync();
                var zones = await _context.Zones.CountAsync(z => z.IsDeleted != true);
                var racks = await _context.Racks.CountAsync(r => r.IsDeleted != true);
                var bins = await _context.Bins.CountAsync(b => b.IsDeleted != true);

                return Json(new
                {
                    total = list.Count,
                    active = list.Count(w => w.IsActive == true),
                    inactive = list.Count(w => w.IsActive != true),
                    addedThisMonth = list.Count(w =>
                        w.CreatedDate.HasValue &&
                        w.CreatedDate.Value.Month == DateTime.Now.Month &&
                        w.CreatedDate.Value.Year == DateTime.Now.Year),
                    zones,
                    racks,
                    bins
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0, zones = 0, racks = 0, bins = 0 });
            }
        }

        // ============================================================
        // GET ALL WAREHOUSES
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.Warehouses
                    .Include(w => w.Branch)
                    .Where(w => w.IsDeleted != true)
                    .OrderBy(w => w.WarehouseId)
                    .Select(w => new
                    {
                        warehouseId = w.WarehouseId,
                        warehouseName = w.WarehouseName,
                        warehouseCode = w.WarehouseCode ?? "",
                        branchName = w.Branch != null ? w.Branch.BranchName : "N/A",
                        address = w.Address ?? "",
                        phone = w.Phone ?? "",
                        email = w.Email ?? "",
                        isActive = w.IsActive ?? false,
                        createdDate = w.CreatedDate,
                        zoneCount = _context.Zones.Count(z => z.WarehouseId == w.WarehouseId && z.IsDeleted != true),
                        rackCount = _context.Racks.Count(r => r.Zone.WarehouseId == w.WarehouseId && r.IsDeleted != true),
                        binCount = _context.Bins.Count(b => b.Rack.Zone.WarehouseId == w.WarehouseId && b.IsDeleted != true)
                    })
                    .ToListAsync();

                return Json(list);
            }
            catch
            {
                return Json(new System.Collections.Generic.List<object>());
            }
        }

        // ============================================================
        // BRANCH DROPDOWN
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var list = await _context.Branches
                    .Where(b => b.IsActive == true && b.IsDeleted != true)
                    .Select(b => new { branchId = b.BranchId, branchName = b.BranchName })
                    .ToListAsync();

                return Json(list);
            }
            catch
            {
                return Json(new System.Collections.Generic.List<object>());
            }
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddWarehousePartial() =>
            PartialView("_RegisterWarehouse", new RegisterWarehouseViewModel());

        [HttpGet]
        public async Task<IActionResult> EditWarehousePartial(int id)
        {
            var w = await _context.Warehouses.FindAsync(id);
            if (w == null) return NotFound();

            var branches = await _context.Branches
                .Where(b => b.IsDeleted != true)
                .Select(b => new BranchSelectItem
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName
                })
                .ToListAsync();

            return PartialView("_EditWarehouse", new EditWarehouseViewModel
            {
                WarehouseId = w.WarehouseId,
                WarehouseName = w.WarehouseName,
                WarehouseCode = w.WarehouseCode ?? "",
                BranchId = w.BranchId,
                Address = w.Address ?? "",
                Phone = w.Phone ?? "",
                Email = w.Email ?? "",
                IsActive = w.IsActive ?? false,
                Branches = branches
            });
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string warehouseName, string warehouseCode, int branchId,
            string address, string phone, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(warehouseName))
                    return Json(new { success = false, message = "Warehouse name required." });

                if (branchId <= 0)
                    return Json(new { success = false, message = "Please select a branch." });

                var dup = await _context.Warehouses.AnyAsync(w =>
                    w.WarehouseName == warehouseName &&
                    w.BranchId == branchId &&
                    w.IsDeleted != true);

                if (dup)
                    return Json(new { success = false, message = "Warehouse already exists in this branch." });

                _context.Warehouses.Add(new ERPWMS.Database.Models.Warehouse
                {
                    WarehouseName = warehouseName.Trim(),
                    WarehouseCode = string.IsNullOrWhiteSpace(warehouseCode) ? null : warehouseCode.Trim(),
                    BranchId = branchId,
                    Address = address ?? "",
                    Phone = phone ?? "",
                    Email = email ?? "",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Warehouse registered successfully!" });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                    return Json(new { success = false, message = "Warehouse code already exists." });

                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int warehouseId, string warehouseName, string warehouseCode, int branchId,
            string address, string phone, string email, bool isActive = true)
        {
            try
            {
                var w = await _context.Warehouses.FindAsync(warehouseId);
                if (w == null) return Json(new { success = false, message = "Not found" });

                w.WarehouseName = warehouseName.Trim();
                w.WarehouseCode = string.IsNullOrWhiteSpace(warehouseCode) ? null : warehouseCode.Trim();
                w.BranchId = branchId;
                w.Address = address ?? "";
                w.Phone = phone ?? "";
                w.Email = email ?? "";
                w.IsActive = isActive;
                w.UpdatedBy = Uid().ToString();
                w.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Warehouse updated successfully!" });
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
                var w = await _context.Warehouses.FindAsync(id);
                if (w == null) return Json(new { success = false, message = "Not found" });

                w.IsDeleted = true;
                w.IsActive = false;
                w.DeletedBy = Uid().ToString();
                w.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Warehouse deleted." });
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
                var w = await _context.Warehouses.FindAsync(id);
                if (w == null) return Json(new { success = false, message = "Not found" });

                w.IsActive = !(w.IsActive ?? false);
                w.UpdatedBy = Uid().ToString();
                w.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = w.IsActive == true ? "Activated." : "Deactivated." });
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
            var w = await _context.Warehouses
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.WarehouseId == id);

            if (w == null) return NotFound();

            ViewBag.BranchName = w.Branch != null ? w.Branch.BranchName : "N/A";
            ViewBag.ZoneCount = await _context.Zones.CountAsync(z => z.WarehouseId == id && z.IsDeleted != true);
            ViewBag.RackCount = await _context.Racks.CountAsync(r => r.Zone.WarehouseId == id && r.IsDeleted != true);
            ViewBag.BinCount = await _context.Bins.CountAsync(b => b.Rack.Zone.WarehouseId == id && b.IsDeleted != true);

            return View(w);
        }

        // ============================================================
        // ZONES
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetZones(int warehouseId)
        {
            var l = await _context.Zones
                .Where(z => z.WarehouseId == warehouseId && z.IsDeleted != true)
                .OrderBy(z => z.ZoneId)
                .Select(z => new
                {
                    zoneId = z.ZoneId,
                    zoneName = z.ZoneName,
                    zoneCode = z.ZoneCode ?? "",
                    isActive = z.IsActive ?? false,
                    createdDate = z.CreatedDate
                })
                .ToListAsync();

            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetZonesForDropdown(int warehouseId)
        {
            var l = await _context.Zones
                .Where(z => z.WarehouseId == warehouseId && z.IsActive == true && z.IsDeleted != true)
                .Select(z => new { zoneId = z.ZoneId, zoneName = z.ZoneName })
                .ToListAsync();

            return Json(l);
        }

        [HttpGet]
        public IActionResult AddZonePartial(int warehouseId)
        {
            ViewBag.WarehouseId = warehouseId;
            return PartialView("_AddZoneForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditZonePartial(int id)
        {
            var z = await _context.Zones.FindAsync(id);
            if (z == null) return NotFound();
            return PartialView("_EditZoneForm", z);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterZone(int warehouseId, string zoneName, string zoneCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(zoneName))
                    return Json(new { success = false, message = "Zone name required." });

                if (warehouseId <= 0)
                    return Json(new { success = false, message = "Invalid warehouse." });

                var d = await _context.Zones.AnyAsync(z =>
                    z.WarehouseId == warehouseId && z.ZoneName == zoneName && z.IsDeleted != true);

                if (d)
                    return Json(new { success = false, message = "Zone already exists in this warehouse." });

                _context.Zones.Add(new Zone
                {
                    WarehouseId = warehouseId,
                    ZoneName = zoneName.Trim(),
                    ZoneCode = string.IsNullOrWhiteSpace(zoneCode) ? null : zoneCode.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditZone(int zoneId, string zoneName, string zoneCode, bool isActive = true)
        {
            try
            {
                var z = await _context.Zones.FindAsync(zoneId);
                if (z == null) return Json(new { success = false, message = "Not found" });

                z.ZoneName = zoneName.Trim();
                z.ZoneCode = string.IsNullOrWhiteSpace(zoneCode) ? null : zoneCode.Trim();
                z.IsActive = isActive;
                z.UpdatedBy = Uid().ToString();
                z.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteZone(int id)
        {
            try
            {
                var z = await _context.Zones.FindAsync(id);
                if (z == null) return Json(new { success = false, message = "Not found" });

                z.IsDeleted = true;
                z.IsActive = false;
                z.DeletedBy = Uid().ToString();
                z.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleZoneActive(int id)
        {
            try
            {
                var z = await _context.Zones.FindAsync(id);
                if (z == null) return Json(new { success = false, message = "Not found" });

                z.IsActive = !(z.IsActive ?? false);
                z.UpdatedBy = Uid().ToString();
                z.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = z.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // RACKS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetRacks(int warehouseId, int? zoneId)
        {
            var q = _context.Racks.Include(x => x.Zone).Where(x => x.IsDeleted != true);

            if (zoneId.HasValue && zoneId > 0)
                q = q.Where(x => x.ZoneId == zoneId);
            else
                q = q.Where(x => x.Zone.WarehouseId == warehouseId);

            var l = await q.OrderBy(x => x.RackId)
                .Select(x => new
                {
                    rackId = x.RackId,
                    rackName = x.RackName,
                    rackCode = x.RackCode ?? "",
                    zoneName = x.Zone != null ? x.Zone.ZoneName : "N/A",
                    isActive = x.IsActive ?? false,
                    createdDate = x.CreatedDate
                })
                .ToListAsync();

            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetRacksForZone(int zoneId)
        {
            var l = await _context.Racks
                .Where(x => x.ZoneId == zoneId && x.IsActive == true && x.IsDeleted != true)
                .Select(x => new { rackId = x.RackId, rackName = x.RackName })
                .ToListAsync();

            return Json(l);
        }

        [HttpGet]
        public IActionResult AddRackPartial(int warehouseId)
        {
            ViewBag.WarehouseId = warehouseId;
            return PartialView("_AddRackForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditRackPartial(int id)
        {
            var x = await _context.Racks
                .Include(r => r.Zone)
                .FirstOrDefaultAsync(r => r.RackId == id);

            if (x == null) return NotFound();
            return PartialView("_EditRackForm", x);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterRack(int warehouseId, int zoneId, string rackName, string rackCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rackName))
                    return Json(new { success = false, message = "Rack name required." });

                if (zoneId <= 0)
                    return Json(new { success = false, message = "Please select a zone." });

                var d = await _context.Racks.AnyAsync(x =>
                    x.ZoneId == zoneId && x.RackName == rackName && x.IsDeleted != true);

                if (d)
                    return Json(new { success = false, message = "Rack already exists in this zone." });

                _context.Racks.Add(new Rack
                {
                    ZoneId = zoneId,
                    RackName = rackName.Trim(),
                    RackCode = string.IsNullOrWhiteSpace(rackCode) ? null : rackCode.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRack(int rackId, string rackName, string rackCode, bool isActive = true)
        {
            try
            {
                var x = await _context.Racks.FindAsync(rackId);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.RackName = rackName.Trim();
                x.RackCode = string.IsNullOrWhiteSpace(rackCode) ? null : rackCode.Trim();
                x.IsActive = isActive;
                x.UpdatedBy = Uid().ToString();
                x.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRack(int id)
        {
            try
            {
                var x = await _context.Racks.FindAsync(id);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.IsDeleted = true;
                x.IsActive = false;
                x.DeletedBy = Uid().ToString();
                x.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleRackActive(int id)
        {
            try
            {
                var x = await _context.Racks.FindAsync(id);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.IsActive = !(x.IsActive ?? false);
                x.UpdatedBy = Uid().ToString();
                x.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = x.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // BINS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetBins(int warehouseId, int? rackId)
        {
            var q = _context.Bins
                .Include(x => x.Rack).ThenInclude(r => r.Zone)
                .Where(x => x.IsDeleted != true);

            if (rackId.HasValue && rackId > 0)
                q = q.Where(x => x.RackId == rackId);
            else
                q = q.Where(x => x.Rack.Zone.WarehouseId == warehouseId);

            var l = await q.OrderBy(x => x.BinId)
                .Select(x => new
                {
                    binId = x.BinId,
                    binName = x.BinName,
                    binCode = x.BinCode ?? "",
                    rackName = x.Rack != null ? x.Rack.RackName : "N/A",
                    zoneName = x.Rack != null && x.Rack.Zone != null ? x.Rack.Zone.ZoneName : "N/A",
                    isOccupied = x.IsOccupied ?? false,
                    isActive = x.IsActive ?? false,
                    createdDate = x.CreatedDate
                })
                .ToListAsync();

            return Json(l);
        }

        [HttpGet]
        public IActionResult AddBinPartial(int warehouseId)
        {
            ViewBag.WarehouseId = warehouseId;
            return PartialView("_AddBinForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditBinPartial(int id)
        {
            var x = await _context.Bins
                .Include(b => b.Rack).ThenInclude(r => r.Zone)
                .FirstOrDefaultAsync(b => b.BinId == id);

            if (x == null) return NotFound();
            return PartialView("_EditBinForm", x);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBin(int rackId, string binName, string binCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(binName))
                    return Json(new { success = false, message = "Bin name required." });

                if (rackId <= 0)
                    return Json(new { success = false, message = "Please select a rack." });

                var d = await _context.Bins.AnyAsync(x =>
                    x.RackId == rackId && x.BinName == binName && x.IsDeleted != true);

                if (d)
                    return Json(new { success = false, message = "Bin already exists in this rack." });

                _context.Bins.Add(new Bin
                {
                    RackId = rackId,
                    BinName = binName.Trim(),
                    BinCode = string.IsNullOrWhiteSpace(binCode) ? null : binCode.Trim(),
                    IsOccupied = false,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBin(int binId, string binName, string binCode, bool isActive = true)
        {
            try
            {
                var x = await _context.Bins.FindAsync(binId);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.BinName = binName.Trim();
                x.BinCode = string.IsNullOrWhiteSpace(binCode) ? null : binCode.Trim();
                x.IsActive = isActive;
                x.UpdatedBy = Uid().ToString();
                x.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBin(int id)
        {
            try
            {
                var x = await _context.Bins.FindAsync(id);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.IsDeleted = true;
                x.IsActive = false;
                x.DeletedBy = Uid().ToString();
                x.DeletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bin deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleBinActive(int id)
        {
            try
            {
                var x = await _context.Bins.FindAsync(id);
                if (x == null) return Json(new { success = false, message = "Not found" });

                x.IsActive = !(x.IsActive ?? false);
                x.UpdatedBy = Uid().ToString();
                x.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = x.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }
    }
}
