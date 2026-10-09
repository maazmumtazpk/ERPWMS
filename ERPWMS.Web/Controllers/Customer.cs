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
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.Customers.Where(c => c.IsDeleted != true).ToListAsync();
            var now = DateTime.Now;
            return Json(new {
                total = l.Count,
                active = l.Count(c => c.IsActive == true),
                inactive = l.Count(c => c.IsActive != true),
                addedThisMonth = l.Count(c => c.CreatedDate.HasValue && c.CreatedDate.Value.Month == now.Month && c.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Customers
                .Where(c => c.IsDeleted != true)
                .OrderBy(c => c.CustomerId)
                .Select(c => new {
                    customerId = c.CustomerId,
                    customerName = c.CustomerName,
                    customerCode = c.CustomerCode,
                    contactPerson = c.ContactPerson,
                    phone = c.Phone,
                    mobile = c.Mobile,
                    email = c.Email,
                    city = c.City,
                    country = c.Country,
                    isActive = c.IsActive == true
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet] public IActionResult AddCustomerPartial() => PartialView("_AddCustomerForm");

        [HttpGet]
        public async Task<IActionResult> EditCustomerPartial(int id)
        {
            var c = await _context.Customers.FindAsync(id);
            if (c == null) return NotFound();
            return PartialView("_EditCustomerForm", c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string customerName, string customerCode, string contactPerson,
            string phone, string mobile, string email, string website,
            string address, string city, string country, string postalCode,
            string taxId, string paymentTerms,
            decimal? creditLimit, int? creditDays, bool isActive = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerName))
                    return Json(new { success = false, message = "Customer name required" });
                if (string.IsNullOrWhiteSpace(phone))
                    return Json(new { success = false, message = "Phone required" });

                var c = new Customer
                {
                    CustomerName = customerName.Trim(),
                    CustomerCode = string.IsNullOrWhiteSpace(customerCode) ? null : customerCode.Trim(),
                    ContactPerson = string.IsNullOrWhiteSpace(contactPerson) ? null : contactPerson.Trim(),
                    Phone = phone.Trim(),
                    Mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim(),
                    Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                    Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim(),
                    Address = address ?? "",
                    City = string.IsNullOrWhiteSpace(city) ? null : city.Trim(),
                    Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim(),
                    PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim(),
                    TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim(),
                    PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms.Trim(),
                    CreditLimit = creditLimit,
                    CreditDays = creditDays,
                    IsActive = isActive,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.Customers.Add(c);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Customer registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int customerId, string customerName, string customerCode, string contactPerson,
            string phone, string mobile, string email, string website,
            string address, string city, string country, string postalCode,
            string taxId, string paymentTerms,
            decimal? creditLimit, int? creditDays, bool isActive = true)
        {
            try
            {
                var c = await _context.Customers.FindAsync(customerId);
                if (c == null) return Json(new { success = false, message = "Customer not found" });

                c.CustomerName = customerName.Trim();
                c.CustomerCode = string.IsNullOrWhiteSpace(customerCode) ? null : customerCode.Trim();
                c.ContactPerson = string.IsNullOrWhiteSpace(contactPerson) ? null : contactPerson.Trim();
                c.Phone = phone.Trim();
                c.Mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim();
                c.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
                c.Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim();
                c.Address = address ?? "";
                c.City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
                c.Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim();
                c.PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim();
                c.TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim();
                c.PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms.Trim();
                c.CreditLimit = creditLimit;
                c.CreditDays = creditDays;
                c.IsActive = isActive;
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Customer updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var c = await _context.Customers.FindAsync(id);
                if (c == null) return Json(new { success = false, message = "Not found" });
                c.IsDeleted = true;
                c.IsActive = false;
                c.DeletedBy = Uid().ToString();
                c.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Customer deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var c = await _context.Customers.FindAsync(id);
                if (c == null) return Json(new { success = false, message = "Not found" });
                c.IsActive = !(c.IsActive ?? false);
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = c.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var c = await _context.Customers.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }
    }
}
