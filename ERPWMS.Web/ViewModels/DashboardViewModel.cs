using System;
using System.Collections.Generic;

namespace ERPWMS.Web.ViewModels
{
    public class TopProductItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Name { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ActivityItem
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public string? Type { get; set; }
        public string? Reference { get; set; }
        public string? Party { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class BranchSelectItem
    {
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
    }

    public class DashboardViewModel
    {
        public int DateRangeDays { get; set; } = 30;
        public decimal TotalSales { get; set; }
        public decimal TodaySales { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal StockValue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSuppliers { get; set; }
        public int LowStockItems { get; set; }
        public int SystemErrors24h { get; set; }
        public dynamic? Dashboard { get; set; }
        public List<string> TrendLabels { get; set; } = new();
        public List<decimal> SalesTrend { get; set; } = new();
        public List<decimal> PurchaseTrend { get; set; } = new();
        public List<TopProductItem> TopProducts { get; set; } = new();
        public List<ActivityItem> RecentActivity { get; set; } = new();

        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalBranches { get; set; }
        public int TotalProducts { get; set; }
        public int TotalWarehouses { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpense { get; set; }
        public string? UserName { get; set; }
        public string? UserRole { get; set; }       

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
