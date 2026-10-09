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
    public class SupplierController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
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
            var l = await _context.Suppliers.Where(s => s.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(s => s.IsActive == true),
                inactive = l.Count(s => s.IsActive != true),
                addedThisMonth = l.Count(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == DateTime.Now.Month &&
                    s.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Suppliers
                .Where(s => s.IsDeleted != true)
                .OrderBy(s => s.SupplierId)
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    supplierName = s.SupplierName,
                    supplierCode = s.SupplierCode ?? "",
                    contactPerson = s.ContactPerson ?? "",
                    phone = s.Phone,
                    email = s.Email ?? "",
                    city = s.City ?? "",
                    isActive = s.IsActive ?? false,
                    createdDate = s.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddSupplierPartial()
        {
            return PartialView("_AddSupplierForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditSupplierPartial(int id)
        {
            var s = await _context.Suppliers.FindAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditSupplierForm", s);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string supplierName, string? supplierCode, string? contactPerson,
            string? phone, string? mobile, string? email, string? website,
            string? address, string? city, string? state, string? country,
            string? postalCode, string? taxId, string? ntn, string? paymentTerms,
            string? bankName, string? bankAccountNo, string? bankBranch,
            decimal? creditLimit, bool isActive = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supplierName))
                    return Json(new { success = false, message = "Supplier name required." });

                var d = await _context.Suppliers.AnyAsync(s => s.SupplierName == supplierName && s.IsDeleted != true);
                if (d)
                    return Json(new { success = false, message = "Supplier already exists." });

                _context.Suppliers.Add(new Supplier
                {
                    SupplierName = supplierName.Trim(),
                    SupplierCode = string.IsNullOrWhiteSpace(supplierCode) ? null : supplierCode.Trim(),
                    ContactPerson = contactPerson ?? "",
                    Phone = phone ?? "",
                    Mobile = mobile ?? "",
                    Email = email ?? "",
                    Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim(),
                    Address = address ?? "",
                    City = city ?? "",
                    State = string.IsNullOrWhiteSpace(state) ? null : state.Trim(),
                    Country = country ?? "",
                    PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim(),
                    TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim(),
                    NTN = string.IsNullOrWhiteSpace(ntn) ? null : ntn.Trim(),
                    PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms.Trim(),
                    BankName = string.IsNullOrWhiteSpace(bankName) ? null : bankName.Trim(),
                    BankAccountNo = string.IsNullOrWhiteSpace(bankAccountNo) ? null : bankAccountNo.Trim(),
                    BankBranch = string.IsNullOrWhiteSpace(bankBranch) ? null : bankBranch.Trim(),
                    CreditLimit = creditLimit,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Supplier registered!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int supplierId, string supplierName, string? supplierCode, string? contactPerson,
            string? phone, string? mobile, string? email, string? website,
            string? address, string? city, string? state, string? country,
            string? postalCode, string? taxId, string? ntn, string? paymentTerms,
            string? bankName, string? bankAccountNo, string? bankBranch,
            decimal? creditLimit, bool isActive = true)
        {
            try
            {
                var s = await _context.Suppliers.FindAsync(supplierId);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.SupplierName = supplierName.Trim();
                s.SupplierCode = string.IsNullOrWhiteSpace(supplierCode) ? null : supplierCode.Trim();
                s.ContactPerson = contactPerson ?? "";
                s.Phone = phone ?? "";
                s.Mobile = mobile ?? "";
                s.Email = email ?? "";
                s.Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim();
                s.Address = address ?? "";
                s.City = city ?? "";
                s.State = string.IsNullOrWhiteSpace(state) ? null : state.Trim();
                s.Country = country ?? "";
                s.PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim();
                s.TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim();
                s.NTN = string.IsNullOrWhiteSpace(ntn) ? null : ntn.Trim();
                s.PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms.Trim();
                s.BankName = string.IsNullOrWhiteSpace(bankName) ? null : bankName.Trim();
                s.BankAccountNo = string.IsNullOrWhiteSpace(bankAccountNo) ? null : bankAccountNo.Trim();
                s.BankBranch = string.IsNullOrWhiteSpace(bankBranch) ? null : bankBranch.Trim();
                s.CreditLimit = creditLimit;
                s.IsActive = isActive;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Supplier updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
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
                var s = await _context.Suppliers.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsDeleted = true;
                s.IsActive = false;
                s.DeletedBy = Uid().ToString();
                s.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Supplier deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
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
                var s = await _context.Suppliers.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsActive = !(s.IsActive ?? false);
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = s.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ============================================================
        // PROFILE
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var s = await _context.Suppliers.FindAsync(id);
            if (s == null) return NotFound();
            return View(s);
        }
    }
}