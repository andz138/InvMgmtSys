using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Models;

[Index("SKU", Name = "UQ__Products__CA1ECF0D75A1D6CD", IsUnique = true)]
public partial class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Product ID")]
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Product Name is required.")]
    [StringLength(100)]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = null!;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(20)]
    public string? SKU { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [Display(Name = "Unit Price")]
    public decimal Price { get; set; }

    [Display(Name = "Reorder Threshold")]
    public int? ReorderThreshold { get; set; }

    [Display(Name = "Supplier")]
    public int? SupplierID { get; set; }
    
    [Display(Name = "Stock Level")]
    public int StockLevel { get; set; }

    [InverseProperty("Product")]
    public virtual Inventory? Inventory { get; set; }

    [ForeignKey("SupplierID")]
    [InverseProperty("Products")]
    public virtual Supplier? Supplier { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
