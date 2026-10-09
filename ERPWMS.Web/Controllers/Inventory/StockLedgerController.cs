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
    public class StockLedgerController : Controller
    {
        private readonly AppDbContext _context;

        public StockLedgerController(AppDbContext c)
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
            var l = await _context.StockLedgers.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(x => x.Quantity > 0),
                inactive = l.Count(x => x.Quantity <= 0),
                addedThisMonth = l.Count(x =>
                    x.TransactionDate.HasValue &&
                    x.TransactionDate.Value.Month == DateTime.Now.Month &&
                    x.TransactionDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.StockLedgers
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.Bin)
                .OrderBy(s => s.LedgerId)
                .Select(s => new
                {
                    ledgerId = s.LedgerId,
                    productName = s.Product != null ? s.Product.ProductName : "N/A",
                    warehouseName = s.Warehouse != null ? s.Warehouse.WarehouseName : "N/A",
                    binName = s.Bin != null ? s.Bin.BinName : "N/A",
                    transactionType = s.TransactionType,
                    referenceDocNo = s.ReferenceDocNo,
                    quantity = s.Quantity,
                    balance = s.Balance,
                    unitCost = s.UnitCost,
                    transactionDate = s.TransactionDate
                })
                .ToListAsync();
            return Json(l);
        }
    }
}