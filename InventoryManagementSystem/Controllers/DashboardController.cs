using InventoryManagementSystem.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers;

public class DashboardController : Controller
{
    private readonly InventoryMgmtDbContext _context;

    public DashboardController(InventoryMgmtDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetSalesTrends()
    {
        var salesData = await _context.Transactions
            .FromSqlRaw(@"
                SELECT 
                    YEAR(TransactionDate) AS Year, 
                    MONTH(TransactionDate) AS Month, 
                    SUM(Quantity) AS TotalSales
                FROM Transactions
                WHERE TransactionType = 'Sale' AND TransactionDate IS NOT NULL
                GROUP BY YEAR(TransactionDate), MONTH(TransactionDate)
                ORDER BY YEAR(TransactionDate), MONTH(TransactionDate)")
            .Select(t => new
            {
                Year = t.TransactionDate.Value.Year,
                Month = t.TransactionDate.Value.Month,
                TotalSales = t.Quantity
            })
            .ToListAsync();

        var formattedData = salesData.Select(d => new
        {
            Month = $"{d.Year}-{d.Month:D2}",
            TotalSales = d.TotalSales
        });

        return Json(new
        {
            labels = formattedData.Select(d => d.Month),
            values = formattedData.Select(d => d.TotalSales)
        });
    }

    public async Task<IActionResult> GetInventoryDistribution()
    {
        var inventoryData = await _context.Products
            .FromSqlRaw(@"
                SELECT 
                    Category, 
                    SUM(StockLevel) AS Stock
                FROM Products
                GROUP BY Category")
            .Select(p => new
            {
                Category = p.Category,
                Stock = p.StockLevel
            })
            .ToListAsync();

        return Json(new
        {
            labels = inventoryData.Select(d => d.Category),
            values = inventoryData.Select(d => d.Stock)
        });
    }

    public async Task<IActionResult> GetDashboardKPIs()
    {
        var totalSales = await _context.Transactions
            .FromSqlRaw(@"
                SELECT SUM(Quantity) AS TotalSales
                FROM Transactions
                WHERE TransactionType = 'Sale")
            .Select(t => t.Quantity)
            .FirstOrDefaultAsync();

        var lowStockItems = await _context.Products
            .FromSqlRaw(@"
                SELECT ProductName, StockLevel
                FROM Products
                WHERE StockLevel < 10")
            .ToListAsync();

        return Json(new
        {
            totalSales = totalSales,
            lowStockItems = lowStockItems
        });
    }
}
