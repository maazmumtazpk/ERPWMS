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
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext c) { _context = c; }
        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, int? dept, int? desig)
        {
            try
            {
                var q = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Branch)
                    .Include(e => e.Shift)
                    .Where(e => e.IsDeleted != true);

                if (!string.IsNullOrWhiteSpace(search))
                    q = q.Where(e => (e.FirstName + " " + e.LastName).Contains(search) || e.EmployeeCode.Contains(search));
                if (dept.HasValue && dept > 0)
                    q = q.Where(e => e.DepartmentId == dept);
                if (desig.HasValue && desig > 0)
                    q = q.Where(e => e.DesignationId == desig);

                var list = await q.OrderBy(e => e.EmployeeId).ToListAsync();

                // ViewBag counts
                ViewBag.TotalEmployees = await _context.Employees.CountAsync(e => e.IsDeleted != true);
                ViewBag.TotalBankAccounts = await _context.EmployeeBankAccounts.CountAsync();
                ViewBag.Departments = await _context.Departments.Where(d => d.IsDeleted != true).ToListAsync();
                ViewBag.Designations = await _context.Designations.Where(d => d.IsDeleted != true).ToListAsync();
                ViewBag.Branches = await _context.Branches.Where(b => b.IsDeleted != true).ToListAsync();
                ViewBag.Shifts = await _context.Shifts.Where(s => s.IsDeleted != true).ToListAsync();
                ViewBag.SearchTerm = search;
                ViewBag.DeptId = dept;
                ViewBag.DesigId = desig;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return View(new List<Employee>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Departments = await _context.Departments.Where(d => d.IsDeleted != true).ToListAsync();
            ViewBag.Designations = await _context.Designations.Where(d => d.IsDeleted != true).ToListAsync();
            ViewBag.Branches = await _context.Branches.Where(b => b.IsDeleted != true).ToListAsync();
            ViewBag.Shifts = await _context.Shifts.Where(s => s.IsDeleted != true).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsDeleted != true).ToListAsync();

            var nextCode = "EMP-" + (await _context.Employees.CountAsync() + 1).ToString("D3");
            ViewBag.NextEmployeeCode = nextCode;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeCreate")]
        public async Task<IActionResult> RegisterEmployee(Employee model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    return Json(new { success = false, message = "First name required." });
                if (string.IsNullOrWhiteSpace(model.LastName))
                    return Json(new { success = false, message = "Last name required." });

                // Auto-generate employee code if empty
                if (string.IsNullOrWhiteSpace(model.EmployeeCode))
                {
                    var count = await _context.Employees.CountAsync() + 1;
                    model.EmployeeCode = "EMP-" + count.ToString("D3");
                }

                // Check duplicate code
                if (await _context.Employees.AnyAsync(e => e.EmployeeCode == model.EmployeeCode && e.IsDeleted != true))
                    return Json(new { success = false, message = "Employee code already exists." });

                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedBy = Uid().ToString();
                model.CreatedDate = DateTime.Now;

                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Employee registered: " + model.EmployeeCode });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            ViewBag.Departments = await _context.Departments.Where(d => d.IsDeleted != true).ToListAsync();
            ViewBag.Designations = await _context.Designations.Where(d => d.IsDeleted != true).ToListAsync();
            ViewBag.Branches = await _context.Branches.Where(b => b.IsDeleted != true).ToListAsync();
            ViewBag.Shifts = await _context.Shifts.Where(s => s.IsDeleted != true).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsDeleted != true).ToListAsync();
            return View(emp);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeUpdate")]
        public async Task<IActionResult> UpdateEmployee(Employee model)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(model.EmployeeId);
                if (emp == null) return Json(new { success = false, message = "Employee not found." });

                emp.FirstName = model.FirstName;
                emp.LastName = model.LastName;
                emp.FatherName = model.FatherName;
                emp.Cnic = model.Cnic;
                emp.DOB = model.DOB;
                emp.Gender = model.Gender;
                emp.DepartmentId = model.DepartmentId;
                emp.DesignationId = model.DesignationId;
                emp.BranchId = model.BranchId;
                emp.WarehouseId = model.WarehouseId;
                emp.ShiftId = model.ShiftId;
                emp.HireDate = model.HireDate;
                emp.BaseSalary = model.BaseSalary;
                emp.IsActive = model.IsActive;
                emp.UpdatedBy = Uid().ToString();
                emp.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Employee updated." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpGet]
        public async Task<IActionResult> ViewEmployee(int id)
        {
            try
            {
                var emp = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Branch)
                    .Include(e => e.Shift)
                    .Include(e => e.Warehouse)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsDeleted != true);

                if (emp == null) return NotFound();

                // Load related data into ViewBags
                ViewBag.BankAccounts = await _context.EmployeeBankAccounts.Where(b => b.EmployeeId == id).ToListAsync();
                ViewBag.Salaries = await _context.EmployeeSalaries.Where(s => s.EmployeeId == id).ToListAsync();
                ViewBag.Leaves = await _context.EmployeeLeaves.Where(l => l.EmployeeId == id).OrderByDescending(l => l.FromDate).Take(20).ToListAsync();
                ViewBag.Attendances = await _context.EmployeeAttendances.Where(a => a.EmployeeId == id).OrderByDescending(a => a.AttendanceDate).Take(20).ToListAsync();

                try {
                    ViewBag.Payrolls = await _context.EmployeePayrolls.Where(p => p.EmployeeId == id).OrderByDescending(p => p.MonthYear).Take(20).ToListAsync();
                } catch { ViewBag.Payrolls = new List<EmployeePayroll>(); }

                return View("ViewEmployee", emp);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return Json(new { success = false, message = "Not found" });
            emp.IsActive = true;
            emp.UpdatedBy = Uid().ToString();
            emp.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee activated." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return Json(new { success = false, message = "Not found" });
            emp.IsActive = false;
            emp.UpdatedBy = Uid().ToString();
            emp.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee deactivated." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeDelete")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return Json(new { success = false, message = "Not found" });
            emp.IsDeleted = true;
            emp.IsActive = false;
            emp.DeletedBy = Uid().ToString();
            emp.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee deleted." });
        }

        [HttpGet]
        public IActionResult Dashboard() => View();
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            try
            {
                var emp = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Branch)
                    .Include(e => e.Shift)
                    .Include(e => e.Warehouse)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsDeleted != true);

                if (emp == null) return NotFound();

                ViewBag.BankAccounts = await _context.EmployeeBankAccounts.Where(b => b.EmployeeId == id).ToListAsync();
                ViewBag.Salaries = await _context.EmployeeSalaries.Where(s => s.EmployeeId == id).ToListAsync();

                return View("Profile", emp);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            var nextCode = "EMP-" + (await _context.Employees.CountAsync() + 1).ToString("D3");
            ViewBag.NextEmployeeCode = nextCode;
            return PartialView("_AddEmployeeForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            return PartialView("_EditEmployeeForm", emp);
        }

        [HttpGet]
        public async Task<IActionResult> GetDropdowns()
        {
            return Json(new {
                departments = await _context.Departments.Where(d => d.IsDeleted != true).Select(d => new { departmentId = d.DepartmentId, departmentName = d.DepartmentName }).ToListAsync(),
                designations = await _context.Designations.Where(d => d.IsDeleted != true).Select(d => new { designationId = d.DesignationId, designationName = d.DesignationName }).ToListAsync(),
                branches = await _context.Branches.Where(b => b.IsDeleted != true).Select(b => new { branchId = b.BranchId, branchName = b.BranchName }).ToListAsync(),
                warehouses = await _context.Warehouses.Where(w => w.IsDeleted != true).Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName }).ToListAsync(),
                shifts = await _context.Shifts.Where(s => s.IsDeleted != true).Select(s => new { shiftId = s.ShiftId, shiftName = s.ShiftName }).ToListAsync()
            });
        }
    }
}




