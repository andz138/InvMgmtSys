using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    public class TransactionsController : Controller
    {
        private readonly InventoryMgmtDbContext _context;

        public TransactionsController(InventoryMgmtDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                .FromSqlRaw(@"
                    SELECT TOP 20 t.*, p.ProductName 
                    FROM Transactions t
                    JOIN Products p ON t.ProductID = p.ProductID
                    ORDER BY t.TransactionDate DESC")
                .ToListAsync();

            return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FromSqlRaw(@"
                    SELECT t.*, p.ProductName 
                    FROM Transactions t
                    JOIN Products p ON t.ProductID = p.ProductID
                    WHERE t.TransactionID = {0}", id)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var transaction = new Transaction
            {
                TransactionDate = DateTime.Now
            };

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName");
            ViewData["TransactionType"] = new SelectList(new List<string> { "Sale", "Restock" });
            return View(transaction);
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionDate,ProductID,Quantity,TransactionType")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .FromSqlRaw("SELECT * FROM Products WHERE ProductID = {0}", transaction.ProductID)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    UpdateStockLevel(product, transaction.TransactionType, transaction.Quantity ?? 0);
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE Products
                        SET StockLevel = @StockLevel
                        WHERE ProductID = @ProductID",
                        new SqlParameter("@StockLevel", product.StockLevel),
                        new SqlParameter("@ProductID", product.ProductID));
                }

                if (transaction.TransactionType == "Restock")
                {
                    var inventory = await _context.Inventories
                        .FromSqlRaw("SELECT * FROM Inventories WHERE ProductID = {0}", transaction.ProductID)
                        .FirstOrDefaultAsync();

                    if (inventory != null)
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            UPDATE Inventories
                            SET LastRestockDate = @LastRestockDate
                            WHERE ProductID = @ProductID",
                            new SqlParameter("@LastRestockDate", transaction.TransactionDate),
                            new SqlParameter("@ProductID", transaction.ProductID));
                    }
                    else
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            INSERT INTO Inventories (ProductID, LastRestockDate, SupplierID, Location)
                            VALUES (@ProductID, @LastRestockDate, @SupplierID, @Location)",
                            new SqlParameter("@ProductID", transaction.ProductID),
                            new SqlParameter("@LastRestockDate", transaction.TransactionDate),
                            new SqlParameter("@SupplierID", product.SupplierID),
                            new SqlParameter("@Location", "Default Location"));
                    }
                }

                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Transactions (TransactionDate, ProductID, Quantity, TransactionType)
                    VALUES (@TransactionDate, @ProductID, @Quantity, @TransactionType)",
                    new SqlParameter("@TransactionDate", transaction.TransactionDate),
                    new SqlParameter("@ProductID", transaction.ProductID),
                    new SqlParameter("@Quantity", transaction.Quantity),
                    new SqlParameter("@TransactionType", transaction.TransactionType));

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FromSqlRaw("SELECT * FROM Transactions WHERE TransactionID = {0}", id)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,TransactionDate,ProductID,Quantity,TransactionType")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE Transactions
                        SET TransactionDate = @TransactionDate, ProductID = @ProductID, 
                            Quantity = @Quantity, TransactionType = @TransactionType
                        WHERE TransactionID = @TransactionID",
                        new SqlParameter("@TransactionDate", transaction.TransactionDate),
                        new SqlParameter("@ProductID", transaction.ProductID),
                        new SqlParameter("@Quantity", transaction.Quantity),
                        new SqlParameter("@TransactionType", transaction.TransactionType),
                        new SqlParameter("@TransactionID", transaction.TransactionID));

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FromSqlRaw(@"
                    SELECT t.*, p.ProductName 
                    FROM Transactions t
                    JOIN Products p ON t.ProductID = p.ProductID
                    WHERE t.TransactionID = {0}", id)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM Transactions
                WHERE TransactionID = @TransactionID",
                new SqlParameter("@TransactionID", id));

            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions
                .FromSqlRaw("SELECT 1 FROM Transactions WHERE TransactionID = {0}", id)
                .Any();
        }

        private void UpdateStockLevel(Product product, string transactionType, int quantity)
        {
            if (transactionType == "Sale")
            {
                product.StockLevel -= quantity;
            }
            else if (transactionType == "Restock")
            {
                product.StockLevel += quantity;
            }
        }
    }
}
