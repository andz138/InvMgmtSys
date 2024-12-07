using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    public class InventoryController : Controller
    {
        private readonly InventoryMgmtDbContext _context;

        public InventoryController(InventoryMgmtDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var inventoryData = await _context.Inventories
                .FromSqlRaw(@"
                    SELECT i.InventoryEntryID, i.ProductID, i.LastRestockDate, i.SupplierID, i.Location,
                           p.ProductName, s.SupplierName
                    FROM Inventories i
                    JOIN Products p ON i.ProductID = p.ProductID
                    JOIN Suppliers s ON i.SupplierID = s.SupplierID")
                .ToListAsync();

            return View(inventoryData);
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FromSqlRaw(@"
                    SELECT i.InventoryEntryID, i.ProductID, i.LastRestockDate, i.SupplierID, i.Location,
                           p.ProductName, s.SupplierName
                    FROM Inventories i
                    JOIN Products p ON i.ProductID = p.ProductID
                    JOIN Suppliers s ON i.SupplierID = s.SupplierID
                    WHERE i.InventoryEntryID = {0}", id)
                .FirstOrDefaultAsync();

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventory/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID");
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("ProductID,LastRestockDate,SupplierID,Location")] Inventory inventory)
{
    if (ModelState.IsValid)
    {
        DateTime? latestRestockDate = null;

        // Use ADO.NET for ExecuteScalarAsync
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = @"
                SELECT TOP 1 TransactionDate
                FROM Transactions
                WHERE ProductID = @ProductID AND TransactionType = 'Restock'
                ORDER BY TransactionDate DESC";

            command.Parameters.Add(new SqlParameter("@ProductID", inventory.ProductID));

            _context.Database.OpenConnection();

            var result = await command.ExecuteScalarAsync();
            if (result != DBNull.Value && result != null)
            {
                latestRestockDate = (DateTime?)result;
            }

            _context.Database.CloseConnection();
        }

        if (latestRestockDate.HasValue)
        {
            inventory.LastRestockDate = latestRestockDate.Value;
        }

        if (string.IsNullOrEmpty(inventory.Location))
        {
            inventory.Location = "Default Location";
        }

        await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Inventories (ProductID, LastRestockDate, SupplierID, Location)
            VALUES (@ProductID, @LastRestockDate, @SupplierID, @Location)",
            new SqlParameter("@ProductID", inventory.ProductID),
            new SqlParameter("@LastRestockDate", inventory.LastRestockDate ?? (object)DBNull.Value),
            new SqlParameter("@SupplierID", inventory.SupplierID),
            new SqlParameter("@Location", inventory.Location));

        return RedirectToAction(nameof(Index));
    }

    ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", inventory.ProductID);
    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", inventory.SupplierID);
    return View(inventory);
}


        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FromSqlRaw(@"
                    SELECT i.InventoryEntryID, i.ProductID, i.LastRestockDate, i.SupplierID, i.Location,
                           p.ProductName, s.SupplierName
                    FROM Inventories i
                    JOIN Products p ON i.ProductID = p.ProductID
                    JOIN Suppliers s ON i.SupplierID = s.SupplierID
                    WHERE i.InventoryEntryID = {0}", id)
                .FirstOrDefaultAsync();

            if (inventory == null)
            {
                return NotFound();
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", inventory.ProductID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", inventory.SupplierID);
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryEntryID,Location,SupplierID,LastRestockDate")] Inventory inventory)
        {
            if (id != inventory.InventoryEntryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE Inventories
                    SET Location = @Location, SupplierID = @SupplierID, LastRestockDate = @LastRestockDate
                    WHERE InventoryEntryID = @InventoryEntryID",
                    new SqlParameter("@Location", inventory.Location),
                    new SqlParameter("@SupplierID", inventory.SupplierID),
                    new SqlParameter("@LastRestockDate", inventory.LastRestockDate ?? (object)DBNull.Value),
                    new SqlParameter("@InventoryEntryID", inventory.InventoryEntryID));

                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", inventory.SupplierID);
            return View(inventory);
        }

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FromSqlRaw(@"
                    SELECT i.InventoryEntryID, i.ProductID, i.LastRestockDate, i.SupplierID, i.Location,
                           p.ProductName, s.SupplierName
                    FROM Inventories i
                    JOIN Products p ON i.ProductID = p.ProductID
                    JOIN Suppliers s ON i.SupplierID = s.SupplierID
                    WHERE i.InventoryEntryID = {0}", id)
                .FirstOrDefaultAsync();

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM Inventories WHERE InventoryEntryID = @InventoryEntryID",
                new SqlParameter("@InventoryEntryID", id));

            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories
                .FromSqlRaw("SELECT 1 FROM Inventories WHERE InventoryEntryID = {0}", id)
                .Any();
        }
    }
}
