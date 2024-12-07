using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ModelTrainer.Models;

public partial class Supplier
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SupplierID { get; set; }

    [StringLength(100)]
    public string? SupplierName { get; set; }

    [StringLength(100)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100)]
    public string? EmailAddress { get; set; }

    [StringLength(50)]
    public string? Location { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    [InverseProperty("Supplier")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
