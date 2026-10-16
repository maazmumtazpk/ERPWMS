using ERPWMS.Application.Common;
using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;
using ICurrentUserService = ERPWMS.Application.Common.ICurrentUserService;

namespace ERPWMS.Web.Controllers.Sales
{
    [Authorize(Policy = "Sales.View")]
    public class PickingController : Controller
    {
        private readonly IPickingService _pickingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;

        public PickingController(
            IPickingService pickingService,
            ICurrentUserService currentUserService,
            AppDbContext context)
        {
            _pickingService = pickingService;
            _currentUserService = currentUserService;
            _context = context;
        }

        // âœ… FIX: CompanyId int? hai â€” direct use karo
        private int GetCurrentCompanyId()
        {
            return _currentUserService.CompanyId ?? 0;
        }

        // âœ… FIX: UserId string? hai â€” TryParse use karo
        private int GetCurrentUserId()
        {
            if (int.TryParse(_currentUserService.UserId, out var id) && id > 0) return id;
            throw new UnauthorizedAccessException("User identity missing.");
        }

        // ============================================================
        // INDEX
        // ============================================================
        public async Task<IActionResult> Index(
            string searchTerm,
            string status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var companyId = GetCurrentCompanyId();

            var pickings = await _pickingService.GetAllAsync(companyId, false);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();

                pickings = pickings.Where(p =>
                    (p.PickingNo ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (p.Sales != null && p.Sales.SalesOrderNo != null &&
                     p.Sales.SalesOrderNo.ToLower().Contains(searchTerm))
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                pickings = pickings.Where(p => p.Status == status);
            }

            if (fromDate.HasValue)
            {
                pickings = pickings.Where(p => p.PickingDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                pickings = pickings.Where(p => p.PickingDate <= toDate);
            }

            var dtos = pickings.Select(p => new PickingListDto
            {
                PickingId = p.PickingId,
                SalesId = p.SalesId,
                SalesOrderNo = p.Sales != null ? (p.Sales.SalesOrderNo ?? string.Empty) : string.Empty,

                WarehouseId = p.WarehouseId,
                WarehouseName = p.Warehouse != null ? (p.Warehouse.WarehouseName ?? string.Empty) : string.Empty,

                PickingNo = p.PickingNo,
                PickingDate = p.PickingDate,
                Status = p.Status,

                AssignedTo = p.AssignedTo ?? 0,
                AssignedToName = "User #" + (p.AssignedTo ?? 0),

                PickedBy = p.PickedBy ?? string.Empty,
                PickedByName = p.PickedBy ?? string.Empty,

                VerifiedBy = p.VerifiedBy ?? string.Empty,
                VerifiedByName = p.VerifiedBy ?? string.Empty,

                CreatedBy = p.CreatedBy,
                CreatedByName = p.CreatedBy,

                CreatedDate = p.CreatedDate,
                IsDeleted = p.IsDeleted ?? false
            }).ToList();

            ViewBag.Statuses = new[]
            {
                "Draft",
                "Assigned",
                "Picked",
                "Verified",
                "Completed",
                "Cancelled"
            };

            ViewBag.CurrentStatus = status;
            ViewBag.SearchTerm = searchTerm;

            return View(dtos);
        }

        // ============================================================
        // CREATE - GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? salesId)
        {
            var companyId = GetCurrentCompanyId();

            if (salesId.HasValue)
            {
                var sales = await _context.SalesMasters
                    .Include(s => s.SalesDetails)
                    .ThenInclude(sd => sd.Product)
                    .Include(s => s.Warehouse)
                    .ThenInclude(w => w.Branch)
                    .FirstOrDefaultAsync(s =>
                        s.SalesId == salesId &&
                        s.Warehouse.Branch.CompanyId == companyId &&
                        s.IsDeleted != true);

                if (sales != null &&
                    (sales.Status == "Approved" || sales.Status == "Completed"))
                {
                    ViewBag.SalesOrder = sales;

                    ViewBag.SalesDetails = sales.SalesDetails
                        .Where(sd => sd.IsDeleted != true)
                        .ToList();
                }
            }

            ViewBag.Warehouses = await _context.Warehouses
                .Include(w => w.Branch)
                .Where(w =>
                    w.Branch.CompanyId == companyId &&
                    w.IsActive == true &&
                    w.IsDeleted != true)
                .ToListAsync();

            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive == true && u.IsDeleted != true)
                .ToListAsync();

            ViewBag.SalesOrders = await _context.SalesMasters
                .Include(s => s.Customer)
                .Include(s => s.Warehouse)
                .ThenInclude(w => w.Branch)
                .Where(s =>
                    s.Warehouse.Branch.CompanyId == companyId &&
                    (s.Status == "Approved" || s.Status == "Completed") &&
                    s.IsDeleted != true)
                .OrderBy(s => s.SalesId)
                .ToListAsync();

            return View();
        }

        // ============================================================
        // CREATE - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Create")]
        public async Task<IActionResult> Create(CreatePickingListRequest request)
        {
            if (!ModelState.IsValid)
            {
                var companyId = GetCurrentCompanyId();

                ViewBag.Warehouses = await _context.Warehouses
                    .Include(w => w.Branch)
                    .Where(w =>
                        w.Branch.CompanyId == companyId &&
                        w.IsActive == true &&
                        w.IsDeleted != true)
                    .ToListAsync();

                ViewBag.Users = await _context.Users
                    .Where(u => u.IsActive == true && u.IsDeleted != true)
                    .ToListAsync();

                ViewBag.SalesOrders = await _context.SalesMasters
                    .Include(s => s.Customer)
                    .Include(s => s.Warehouse)
                    .ThenInclude(w => w.Branch)
                    .Where(s =>
                        s.Warehouse.Branch.CompanyId == companyId &&
                        (s.Status == "Approved" || s.Status == "Completed") &&
                        s.IsDeleted != true)
                    .OrderBy(s => s.SalesId)
                    .ToListAsync();

                return View(request);
            }

            try
            {
                var currentUserId = GetCurrentUserId();

                var picking = new PickingList
                {
                    SalesId = request.SalesId,
                    WarehouseId = request.WarehouseId,
                    PickingNo = request.PickingNo,
                    PickingDate = request.PickingDate ?? DateTime.UtcNow,
                    AssignedTo = request.AssignedTo,
                    CreatedBy = currentUserId.ToString()
                };

                var details = request.PickingDetails.Select(d => new PickingDetail
                {
                    SalesDetailId = d.SalesDetailId,
                    ProductId = d.ProductId,
                    BinId = d.BinId,
                    PickedQty = d.PickedQty,
                    Status = d.Status ?? "Pending",
                    BatchNo = d.BatchNo,
                    SerialNumbers = d.SerialNumbers,
                    CreatedBy = currentUserId.ToString()
                }).ToList();

                var result = await _pickingService.CreatePickingListAsync(
                    picking,
                    details,
                    currentUserId.ToString());

                TempData["Success"] =
                    $"Picking List '{result.PickingNo}' created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var companyId = GetCurrentCompanyId();

                ViewBag.Warehouses = await _context.Warehouses
                    .Include(w => w.Branch)
                    .Where(w =>
                        w.Branch.CompanyId == companyId &&
                        w.IsActive == true &&
                        w.IsDeleted != true)
                    .ToListAsync();

                ViewBag.Users = await _context.Users
                    .Where(u => u.IsActive == true && u.IsDeleted != true)
                    .ToListAsync();

                ViewBag.SalesOrders = await _context.SalesMasters
                    .Include(s => s.Customer)
                    .Include(s => s.Warehouse)
                    .ThenInclude(w => w.Branch)
                    .Where(s =>
                        s.Warehouse.Branch.CompanyId == companyId &&
                        (s.Status == "Approved" || s.Status == "Completed") &&
                        s.IsDeleted != true)
                    .OrderBy(s => s.SalesId)
                    .ToListAsync();

                return View(request);
            }
        }

        // ============================================================
        // EDIT - GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var companyId = GetCurrentCompanyId();

            var picking = await _pickingService.GetByIdAsync(id, companyId);

            if (picking == null)
                return NotFound();

            if (picking.Status != "Draft")
            {
                TempData["Error"] =
                    "Cannot edit a picking list that is not in Draft status.";

                return RedirectToAction(nameof(Details), new { id });
            }

            var details = await _context.PickingDetails
                .Include(d => d.Product)
                .Include(d => d.Bin)
                .Where(d => d.PickingId == id)
                .ToListAsync();

            var model = new UpdatePickingListRequest
            {
                PickingId = picking.PickingId,
                SalesId = picking.SalesId,
                WarehouseId = picking.WarehouseId,
                PickingNo = picking.PickingNo,
                PickingDate = picking.PickingDate,
                AssignedTo = picking.AssignedTo,
                Status = picking.Status,

                PickingDetails = details.Select(d =>
                    new CreatePickingDetailRequest
                    {
                        SalesDetailId = d.SalesDetailId,
                        ProductId = d.ProductId,
                        BinId = d.BinId,
                        PickedQty = d.PickedQty,
                        Status = d.Status,
                        BatchNo = d.BatchNo,
                        SerialNumbers = d.SerialNumbers
                    }).ToList()
            };

            var companyIdForViews = GetCurrentCompanyId();

            ViewBag.Warehouses = await _context.Warehouses
                .Include(w => w.Branch)
                .Where(w =>
                    w.Branch.CompanyId == companyIdForViews &&
                    w.IsActive == true &&
                    w.IsDeleted != true)
                .ToListAsync();

            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive == true && u.IsDeleted != true)
                .ToListAsync();

            ViewBag.SalesOrders = await _context.SalesMasters
                .Include(s => s.Customer)
                .Include(s => s.Warehouse)
                .ThenInclude(w => w.Branch)
                .Where(s =>
                    s.Warehouse.Branch.CompanyId == companyIdForViews &&
                    (s.Status == "Approved" || s.Status == "Completed") &&
                    s.IsDeleted != true)
                .OrderBy(s => s.SalesId)
                .ToListAsync();

            return View(model);
        }

        // ============================================================
        // EDIT - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Edit")]
        public async Task<IActionResult> Edit(UpdatePickingListRequest request)
        {
            if (!ModelState.IsValid)
            {
                var companyId = GetCurrentCompanyId();

                ViewBag.Warehouses = await _context.Warehouses
                    .Include(w => w.Branch)
                    .Where(w =>
                        w.Branch.CompanyId == companyId &&
                        w.IsActive == true &&
                        w.IsDeleted != true)
                    .ToListAsync();

                ViewBag.Users = await _context.Users
                    .Where(u => u.IsActive == true && u.IsDeleted != true)
                    .ToListAsync();

                ViewBag.SalesOrders = await _context.SalesMasters
                    .Include(s => s.Customer)
                    .Include(s => s.Warehouse)
                    .ThenInclude(w => w.Branch)
                    .Where(s =>
                        s.Warehouse.Branch.CompanyId == companyId &&
                        (s.Status == "Approved" || s.Status == "Completed") &&
                        s.IsDeleted != true)
                    .OrderBy(s => s.SalesId)
                    .ToListAsync();

                return View(request);
            }

            try
            {
                var currentUserId = GetCurrentUserId();

                var picking = await _pickingService.GetByIdAsync(
                    request.PickingId,
                    GetCurrentCompanyId());

                if (picking == null)
                    return NotFound();

                if (picking.Status != "Draft")
                {
                    TempData["Error"] =
                        "Cannot edit a picking list that is not in Draft status.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id = request.PickingId });
                }

                picking.SalesId = request.SalesId;
                picking.WarehouseId = request.WarehouseId;
                picking.PickingNo = request.PickingNo;
                picking.PickingDate =
                    request.PickingDate ?? DateTime.UtcNow;
                picking.AssignedTo = request.AssignedTo;

                var details = request.PickingDetails.Select(d =>
                    new PickingDetail
                    {
                        SalesDetailId = d.SalesDetailId,
                        ProductId = d.ProductId,
                        BinId = d.BinId,
                        PickedQty = d.PickedQty,
                        Status = d.Status ?? "Pending",
                        BatchNo = d.BatchNo,
                        SerialNumbers = d.SerialNumbers
                    }).ToList();

                await _pickingService.UpdatePickingListAsync(
                    picking,
                    details,
                    currentUserId.ToString());

                TempData["Success"] =
                    $"Picking List '{picking.PickingNo}' updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var companyId = GetCurrentCompanyId();

                ViewBag.Warehouses = await _context.Warehouses
                    .Include(w => w.Branch)
                    .Where(w =>
                        w.Branch.CompanyId == companyId &&
                        w.IsActive == true &&
                        w.IsDeleted != true)
                    .ToListAsync();

                ViewBag.Users = await _context.Users
                    .Where(u => u.IsActive == true && u.IsDeleted != true)
                    .ToListAsync();

                ViewBag.SalesOrders = await _context.SalesMasters
                    .Include(s => s.Customer)
                    .Include(s => s.Warehouse)
                    .ThenInclude(w => w.Branch)
                    .Where(s =>
                        s.Warehouse.Branch.CompanyId == companyId &&
                        (s.Status == "Approved" || s.Status == "Completed") &&
                        s.IsDeleted != true)
                    .OrderBy(s => s.SalesId)
                    .ToListAsync();

                return View(request);
            }
        }

        // ============================================================
        // DETAILS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var companyId = GetCurrentCompanyId();

            var picking = await _pickingService.GetByIdAsync(id, companyId);

            if (picking == null)
                return NotFound();

            var details = await _context.PickingDetails
                .Include(d => d.Product)
                .Include(d => d.Bin)
                .Where(d => d.PickingId == id)
                .ToListAsync();

            var model = new PickingListDto
            {
                PickingId = picking.PickingId,
                SalesId = picking.SalesId,

                SalesOrderNo = picking.Sales != null
                    ? (picking.Sales.SalesOrderNo ?? string.Empty)
                    : string.Empty,

                WarehouseId = picking.WarehouseId,

                WarehouseName = picking.Warehouse != null
                    ? (picking.Warehouse.WarehouseName ?? string.Empty)
                    : string.Empty,

                PickingNo = picking.PickingNo,
                PickingDate = picking.PickingDate,
                Status = picking.Status,

                AssignedTo = picking.AssignedTo ?? 0,
                AssignedToName = "User #" + (picking.AssignedTo ?? 0),

                PickedBy = picking.PickedBy ?? string.Empty,
                PickedByName = picking.PickedBy ?? string.Empty,

                VerifiedBy = picking.VerifiedBy ?? string.Empty,
                VerifiedByName = picking.VerifiedBy ?? string.Empty,

                CreatedBy = picking.CreatedBy,
                CreatedByName = picking.CreatedBy,

                CreatedDate = picking.CreatedDate,

                IsDeleted = picking.IsDeleted ?? false,

                PickingDetails = details.Select(d =>
                    new PickingDetailDto
                    {
                        PickingDetailId = d.PickingDetailId,
                        PickingId = d.PickingId,
                        SalesDetailId = d.SalesDetailId,
                        ProductId = d.ProductId,

                        ProductName = d.Product != null
                            ? (d.Product.ProductName ?? string.Empty)
                            : string.Empty,

                        ProductCode = d.Product != null
                            ? (d.Product.Sku ?? string.Empty)
                            : string.Empty,

                        BinId = d.BinId,

                        BinName = d.Bin != null
                            ? (d.Bin.BinName ?? string.Empty)
                            : string.Empty,

                        PickedQty = d.PickedQty,
                        Status = d.Status,
                        BatchNo = d.BatchNo,
                        SerialNumbers = d.SerialNumbers,
                        CreatedBy = d.CreatedBy,
                        CreatedDate = d.CreatedDate
                    }).ToList()
            };

            return View(model);
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _pickingService.DeletePickingListAsync(
                    id,
                    GetCurrentCompanyId(),
                    GetCurrentUserId().ToString());

                TempData["Success"] = "Picking List deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // ASSIGN PICKER
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Edit")]
        public async Task<IActionResult> AssignPicker(int id, int assignedTo)
        {
            try
            {
                await _pickingService.AssignPickerAsync(
                    id,
                    assignedTo,
                    GetCurrentUserId().ToString());

                TempData["Success"] = "Picker assigned successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ============================================================
        // CONFIRM PICKING
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Approve")]
        public async Task<IActionResult> ConfirmPicking(int id)
        {
            try
            {
                await _pickingService.ConfirmPickingAsync(
                    id,
                    GetCurrentUserId().ToString());

                TempData["Success"] = "Picking confirmed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ============================================================
        // VERIFY PICKING
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Approve")]
        public async Task<IActionResult> VerifyPicking(int id)
        {
            try
            {
                await _pickingService.VerifyPickingAsync(
                    id,
                    GetCurrentUserId().ToString());

                TempData["Success"] = "Picking verified.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ============================================================
        // GENERATE DISPATCH
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Create")]
        public async Task<IActionResult> GenerateDispatch(int id)
        {
            try
            {
                var dispatch =
                    await _pickingService.GenerateDispatchFromPickingAsync(
                        id,
                        GetCurrentUserId().ToString());

                TempData["Success"] =
                    $"Dispatch '{dispatch.DispatchNo}' generated successfully.";

                return RedirectToAction(
                    "Details",
                    "Dispatch",
                    new { id = dispatch.DispatchId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();

                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
