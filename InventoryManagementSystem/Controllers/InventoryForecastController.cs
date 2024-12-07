using Microsoft.Extensions.ML;
using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class InventoryForecastController : Controller
    {
        private readonly PredictionEnginePool<ProductSalesData, ProductSalesPrediction> _predictionEnginePool;
        private readonly InventoryMgmtDbContext _context;

        public InventoryForecastController(
            PredictionEnginePool<ProductSalesData, ProductSalesPrediction> predictionEnginePool,
            InventoryMgmtDbContext context)
        {
            _predictionEnginePool = predictionEnginePool;
            _context = context;
        }

        public async Task<IActionResult> ForecastInventory(int productId)
        {
            DateTime? latestDate = null;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                    SELECT MAX(TransactionDate)
                    FROM Transactions
                    WHERE TransactionType = 'Sale' AND ProductID = @ProductID";
                command.Parameters.Add(new SqlParameter("@ProductID", productId));

                _context.Database.OpenConnection();

                var result = await command.ExecuteScalarAsync();
                if (result != DBNull.Value && result != null)
                {
                    latestDate = (DateTime?)result;
                }

                _context.Database.CloseConnection();
            }

            if (latestDate == null)
            {
                return NotFound("No sales data available for this product.");
            }

            var inputData = await PrepareInputData(productId, latestDate.Value.Date);

            var prediction = _predictionEnginePool.Predict(inputData);

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound($"Product with ID {productId} not found.");
            }

            ViewBag.PredictedQuantity = prediction.Score;

            return View(product);
        }

        private async Task<ProductSalesData> PrepareInputData(int productId, DateTime date)
        {
            float? lag1 = await GetLag(productId, date, 1);
            float? lag7 = await GetLag(productId, date, 7);
            float? lag14 = await GetLag(productId, date, 14);

            if (lag1 == null || lag7 == null || lag14 == null)
            {
                throw new InvalidOperationException("Insufficient data to generate prediction.");
            }

            return new ProductSalesData
            {
                Lag1 = lag1.Value,
                Lag7 = lag7.Value,
                Lag14 = lag14.Value,
                DayOfWeek = (float)date.DayOfWeek,
                Month = date.Month,
                Year = date.Year,
                ProductId = productId.ToString()
            };
        }

        private async Task<float?> GetLag(int productId, DateTime date, int lag)
        {
            DateTime lagDate = date.AddDays(-lag);
            float? quantitySold = null;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                    SELECT SUM(CAST(Quantity AS FLOAT))
                    FROM Transactions
                    WHERE TransactionType = 'Sale' 
                      AND ProductID = @ProductID 
                      AND CAST(TransactionDate AS DATE) = @LagDate";
                command.Parameters.Add(new SqlParameter("@ProductID", productId));
                command.Parameters.Add(new SqlParameter("@LagDate", lagDate));

                _context.Database.OpenConnection();

                var result = await command.ExecuteScalarAsync();
                if (result != DBNull.Value && result != null)
                {
                    quantitySold = (float?)result;
                }

                _context.Database.CloseConnection();
            }

            return quantitySold;
        }
    }
}
