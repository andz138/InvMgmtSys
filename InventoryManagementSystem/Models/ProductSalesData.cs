namespace InventoryManagementSystem.Models;

public class ProductSalesData
{
    public float QuantitySold { get; set; }
    public float Lag1 { get; set; }
    public float Lag7 { get; set; }
    public float Lag14 { get; set; }
    public float DayOfWeek { get; set; }
    public float Month { get; set; }
    public float Year { get; set; }
    public string ProductId { get; set; }
}