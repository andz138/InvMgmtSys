using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    public class SuppliersController : Controller
    {
        private readonly InventoryMgmtDbContext _context;

        public SuppliersController(InventoryMgmtDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM Suppliers")
                .ToListAsync();

            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM Suppliers WHERE SupplierID = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierName,PhoneNumber,EmailAddress,Location")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO Suppliers (SupplierName, PhoneNumber, EmailAddress, Location)
                    VALUES (@SupplierName, @PhoneNumber, @EmailAddress, @Location)",
                    new SqlParameter("@SupplierName", supplier.SupplierName),
                    new SqlParameter("@PhoneNumber", supplier.PhoneNumber),
                    new SqlParameter("@EmailAddress", supplier.EmailAddress),
                    new SqlParameter("@Location", supplier.Location));

                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM Suppliers WHERE SupplierID = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierID,SupplierName,PhoneNumber,EmailAddress,Location")] Supplier supplier)
        {
            if (id != supplier.SupplierID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE Suppliers
                        SET SupplierName = @SupplierName, PhoneNumber = @PhoneNumber, 
                            EmailAddress = @EmailAddress, Location = @Location
                        WHERE SupplierID = @SupplierID",
                        new SqlParameter("@SupplierName", supplier.SupplierName),
                        new SqlParameter("@PhoneNumber", supplier.PhoneNumber),
                        new SqlParameter("@EmailAddress", supplier.EmailAddress),
                        new SqlParameter("@Location", supplier.Location),
                        new SqlParameter("@SupplierID", supplier.SupplierID));

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM Suppliers WHERE SupplierID = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM Suppliers
                WHERE SupplierID = @SupplierID",
                new SqlParameter("@SupplierID", id));

            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers
                .FromSqlRaw("SELECT 1 FROM Suppliers WHERE SupplierID = {0}", id)
                .Any();
        }
    }
}
