using ModelTrainer.Context;
using ModelTrainer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ModelTrainer.DataProcessing
{
    public class DataExtractor
    {
        private readonly InventoryMgmtDbContext _context;

        public DataExtractor(InventoryMgmtDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesData>> GetSalesDataAsync()
        {
            var salesData = await _context.SalesData
                .FromSqlRaw(@"
                    SELECT 
                        CAST(TransactionDate AS DATE) AS Date,
                        ProductID,
                        SUM(COALESCE(Quantity, 0)) AS QuantitySold
                    FROM Transactions
                    WHERE TransactionType = 'Sale' AND TransactionDate IS NOT NULL
                    GROUP BY CAST(TransactionDate AS DATE), ProductID
                    ORDER BY CAST(TransactionDate AS DATE)")
                .ToListAsync();

            return salesData;
        }
    }
}