using Microsoft.EntityFrameworkCore;
using ModelTrainer.Context;
using ModelTrainer.DataProcessing;

namespace ModelTrainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InventoryMgmtDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryManagement;User Id=sa;Password=h9KnZXp1;TrustServerCertificate=True;");

            using var context = new InventoryMgmtDbContext(optionsBuilder.Options);

            // Extract data
            var dataExtractor = new DataExtractor(context);
            var salesData = await dataExtractor.GetSalesDataAsync();

            // Train and save the model
            var modelTrainer = new MachineLearning.ModelTrainer();
            modelTrainer.TrainAndSaveModel(salesData);

            Console.WriteLine("Model training completed.");
        }
    }
}