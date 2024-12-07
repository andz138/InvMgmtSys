using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryMgmtDbContext _context;

        public ProductsController(InventoryMgmtDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .FromSqlRaw(@"
                    SELECT p.*, s.SupplierName 
                    FROM Products p
                    JOIN Suppliers s ON p.SupplierID = s.SupplierID")
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FromSqlRaw(@"
                    SELECT p.*, s.SupplierName 
                    FROM Products p
                    JOIN Suppliers s ON p.SupplierID = s.SupplierID
                    WHERE p.ProductID = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Category,SKU,Price,ReorderThreshold,SupplierID")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO Products (ProductName, Category, SKU, Price, ReorderThreshold, SupplierID)
                        VALUES (@ProductName, @Category, @SKU, @Price, @ReorderThreshold, @SupplierID)",
                        new SqlParameter("@ProductName", product.ProductName),
                        new SqlParameter("@Category", product.Category),
                        new SqlParameter("@SKU", product.SKU),
                        new SqlParameter("@Price", product.Price),
                        new SqlParameter("@ReorderThreshold", product.ReorderThreshold),
                        new SqlParameter("@SupplierID", product.SupplierID));

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is SqlException sqlEx && sqlEx.Message.Contains("UQ__Products"))
                    {
                        ModelState.AddModelError("SKU", "A product with this SKU already exists. Please choose a unique SKU.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the product. Please try again.");
                    }
                }
            }

            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FromSqlRaw(@"
                    SELECT * 
                    FROM Products
                    WHERE ProductID = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Category,SKU,Price,ReorderThreshold,SupplierID")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE Products
                        SET ProductName = @ProductName, Category = @Category, SKU = @SKU, 
                            Price = @Price, ReorderThreshold = @ReorderThreshold, SupplierID = @SupplierID
                        WHERE ProductID = @ProductID",
                        new SqlParameter("@ProductName", product.ProductName),
                        new SqlParameter("@Category", product.Category),
                        new SqlParameter("@SKU", product.SKU),
                        new SqlParameter("@Price", product.Price),
                        new SqlParameter("@ReorderThreshold", product.ReorderThreshold),
                        new SqlParameter("@SupplierID", product.SupplierID),
                        new SqlParameter("@ProductID", product.ProductID));

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FromSqlRaw(@"
                    SELECT p.*, s.SupplierName 
                    FROM Products p
                    JOIN Suppliers s ON p.SupplierID = s.SupplierID
                    WHERE p.ProductID = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM Products
                WHERE ProductID = @ProductID",
                new SqlParameter("@ProductID", id));

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products
                .FromSqlRaw(@"
                    SELECT 1 
                    FROM Products 
                    WHERE ProductID = {0}", id)
                .Any();
        }
    }
}
