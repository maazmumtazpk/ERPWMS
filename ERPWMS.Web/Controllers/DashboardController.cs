using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Web.ViewModels;

namespace ERPWMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int days = 30)
        {
            var vm = new DashboardViewModel { DateRangeDays = days };
            var from = DateTime.Today.AddDays(-days);

            // ---- SALES ----
            try
            {
                vm.TotalSales = await _context.SalesMasters
                    .SumAsync(s => (decimal?)s.NetAmount) ?? 0m;

                vm.TodaySales = await _context.SalesMasters
                    .Where(s => s.OrderDate.HasValue && s.OrderDate.Value.Date == DateTime.Today)
                    .SumAsync(s => (decimal?)s.NetAmount) ?? 0m;

                vm.TotalOrders = await _context.SalesMasters.CountAsync();

                vm.PendingOrders = await _context.SalesMasters
                    .CountAsync(s => s.Status == null ||
                                     (s.Status != "Completed" &&
                                      s.Status != "Delivered" &&
                                      s.Status != "Closed"));
            }
            catch { }

            // ---- STOCK VALUE ----
            try
            {
                vm.StockValue = await _context.InventoryValuations
                    .SumAsync(v => (decimal?)v.TotalValue) ?? 0m;
            }
            catch { }

            // ---- COUNTS ----
            try { vm.TotalCustomers = await _context.Customers.CountAsync(); } catch { }
            try { vm.TotalProducts = await _context.Products.CountAsync(); } catch { }
            try { vm.TotalSuppliers = await _context.Suppliers.CountAsync(); } catch { }
            try { vm.TotalWarehouses = await _context.Warehouses.CountAsync(); } catch { }

            // ---- LOW STOCK ----
            try
            {
                vm.LowStockItems = await _context.Products
                    .Where(p => p.IsActive == true &&
                                p.ReorderLevel.HasValue &&
                                p.ReorderLevel.Value > 0)
                    .CountAsync();
            }
            catch { }

            // ---- ERRORS 24H ----
            try
            {
                var since = DateTime.Now.AddHours(-24);
                vm.SystemErrors24h = await _context.ErrorLogs
                    .CountAsync(e => e.ErrorDate.HasValue && e.ErrorDate.Value >= since);
            }
            catch { }

            // ---- TREND ----
            try
            {
                var salesByDay = await _context.SalesMasters
                    .Where(s => s.OrderDate.HasValue && s.OrderDate.Value >= from)
                    .GroupBy(s => s.OrderDate!.Value.Date)
                    .Select(g => new { Day = g.Key, Total = g.Sum(x => x.NetAmount) })
                    .ToListAsync();

                var purchByDay = await _context.PurchaseMasters
                    .Where(p => p.OrderDate.HasValue && p.OrderDate.Value >= from)
                    .GroupBy(p => p.OrderDate!.Value.Date)
                    .Select(g => new { Day = g.Key, Total = g.Sum(x => x.NetAmount) })
                    .ToListAsync();

                for (int i = 0; i <= days; i++)
                {
                    var d = from.AddDays(i);
                    vm.TrendLabels.Add(d.ToString("dd MMM"));
                    vm.SalesTrend.Add(salesByDay.FirstOrDefault(x => x.Day == d)?.Total ?? 0m);
                    vm.PurchaseTrend.Add(purchByDay.FirstOrDefault(x => x.Day == d)?.Total ?? 0m);
                }
            }
            catch { }

            // ---- TOP PRODUCTS ----
            try
            {
                var top = await _context.SalesDetails
                    .GroupBy(d => d.ProductId)
                    .Select(g => new { ProductId = g.Key, Revenue = g.Sum(x => x.TotalPrice) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(5)
                    .ToListAsync();

                foreach (var t in top)
                {
                    var prod = await _context.Products.FindAsync(t.ProductId);
                    if (prod != null)
                    {
                        vm.TopProducts.Add(new TopProductItem
                        {
                            ProductId = prod.ProductId,
                            Name = prod.ProductName,
                            ProductName = prod.ProductName,
                            Revenue = t.Revenue
                        });
                    }
                }
            }
            catch { }

            // ---- RECENT ACTIVITY ----
            try
            {
                var recentSales = await _context.SalesMasters
                    .OrderByDescending(s => s.OrderDate)
                    .Take(6)
                    .Select(s => new ActivityItem
                    {
                        Type = "Sales",
                        Reference = s.SalesOrderNo,
                        Party = s.Customer != null ? s.Customer.CustomerName : "-",
                        Amount = s.NetAmount,
                        Status = s.Status ?? "Pending",
                        Date = s.OrderDate ?? DateTime.Now
                    })
                    .ToListAsync();

                var recentPurch = await _context.PurchaseMasters
                    .OrderByDescending(p => p.OrderDate)
                    .Take(6)
                    .Select(p => new ActivityItem
                    {
                        Type = "Purchase",
                        Reference = p.PurchaseOrderNo,
                        Party = p.Supplier != null ? p.Supplier.SupplierName : "-",
                        Amount = p.NetAmount,
                        Status = p.Status ?? "Pending",
                        Date = p.OrderDate ?? DateTime.Now
                    })
                    .ToListAsync();

                vm.RecentActivity = recentSales
                    .Concat(recentPurch)
                    .OrderByDescending(a => a.Date)
                    .Take(10)
                    .ToList();
            }
            catch { }

            vm.LastUpdated = DateTime.Now;
            return View(vm);
        }
    }
}