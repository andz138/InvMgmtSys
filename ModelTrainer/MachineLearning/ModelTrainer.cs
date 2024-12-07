using Microsoft.ML;
using ModelTrainer.Models;


namespace ModelTrainer.MachineLearning
{
    public class ModelTrainer
    {
        private readonly MLContext _mlContext;

        public ModelTrainer()
        {
            _mlContext = new MLContext();
        }

        public void TrainAndSaveModel(List<SalesData> salesData)
        {
            var enrichedData = PrepareTrainingData(salesData);

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(enrichedData);

            // Define training pipeline
            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("ProductId")
                .Append(_mlContext.Transforms.Concatenate("Features", "Lag1", "Lag7", "Lag14", "DayOfWeek", "Month", "Year", "ProductId"))
                // Specify "QuantitySold" as the label column and "Features" as the feature column
                .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "QuantitySold", featureColumnName: "Features"));
            // Train the model
            var model = pipeline.Fit(dataView);
            // Evaluate the model
            var predictions = model.Transform(dataView);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "QuantitySold");
            Console.WriteLine($"R^2: {metrics.RSquared}");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError}");
            // Save the model
            _mlContext.Model.Save(model, dataView.Schema, "InventoryForecastModel.zip");
        }

        private List<ProductSalesData> PrepareTrainingData(List<SalesData> salesData)
        {
            var enrichedData = new List<ProductSalesData>();
            var dataByProduct = salesData.GroupBy(s => s.ProductId);

            foreach (var productGroup in dataByProduct)
            {
                var productSales = productGroup.OrderBy(s => s.Date).ToList();

                for (int i = 14; i < productSales.Count; i++)
                {
                    var currentData = productSales[i];
                    var lag1 = productSales[i - 1].QuantitySold;
                    var lag7 = productSales[i - 7].QuantitySold;
                    var lag14 = productSales[i - 14].QuantitySold;

                    enrichedData.Add(new ProductSalesData
                    {
                        QuantitySold = currentData.QuantitySold,
                        Lag1 = lag1,
                        Lag7 = lag7,
                        Lag14 = lag14,
                        DayOfWeek = (float)currentData.Date.DayOfWeek,
                        Month = currentData.Date.Month,
                        Year = currentData.Date.Year,
                        ProductId = currentData.ProductId.ToString()
                    });
                }
            }
            return enrichedData;
        }
    }
}
