using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ModelTrainer.Models;

[Table("Inventory")]
public partial class Inventory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InventoryEntryID { get; set; }
    public int ProductID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastRestockDate { get; set; }

    public int? SupplierID { get; set; }
    
    [StringLength(50)]
    public string? Location { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("Inventory")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("SupplierID")]
    [InverseProperty("Inventories")]
    public virtual Supplier? Supplier { get; set; }
}
