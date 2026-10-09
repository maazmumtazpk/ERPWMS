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

namespace ERPWMS.Web.Controllers.Inventory
{
    [Authorize]
    public class SerialNumberController : Controller
    {
        private readonly AppDbContext _context;

        public SerialNumberController(AppDbContext c)
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
            var l = await _context.SerialNumbers.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(s => s.Status == "InStock" || s.Status == "Available" || s.Status == null),
                inactive = l.Count(s => s.Status == "Sold" || s.Status == "Dispatched"),
                addedThisMonth = l.Count(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == DateTime.Now.Month &&
                    s.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.SerialNumbers
                .Include(s => s.Product)
                .OrderBy(s => s.SerialId)
                .Select(s => new
                {
                    serialId = s.SerialId,
                    serialNo = s.SerialNo,
                    productName = s.Product != null ? s.Product.ProductName : "N/A",
                    status = s.Status ?? "InStock",
                    outDate = s.OutDate,
                    createdDate = s.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET PRODUCTS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var l = await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddSerialPartial() => PartialView("_AddSerialForm");

        [HttpGet]
        public async Task<IActionResult> EditSerialPartial(int id)
        {
            var s = await _context.SerialNumbers.FindAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditSerialForm", s);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int productId,
            int grnDetailId,
            string serialNo,
            string? status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serialNo))
                    return Json(new { success = false, message = "Serial No required." });
                if (productId <= 0)
                    return Json(new { success = false, message = "Please select product." });

                _context.SerialNumbers.Add(new SerialNumber
                {
                    ProductId = productId,
                    GrndetailId = grnDetailId,
                    SerialNo = serialNo.Trim(),
                    Status = status ?? "InStock",
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Serial number added!" });
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
            int serialId,
            string serialNo,
            string? status)
        {
            try
            {
                var s = await _context.SerialNumbers.FindAsync(serialId);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.SerialNo = serialNo.Trim();
                s.Status = status;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Serial updated!" });
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
                var s = await _context.SerialNumbers.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.DeletedBy = Uid().ToString();
                s.DeletedDate = DateTime.Now;
                _context.SerialNumbers.Remove(s);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}