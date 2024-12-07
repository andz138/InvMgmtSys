using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ModelTrainer.Models;

public partial class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TransactionDate { get; set; }

    public int? ProductID { get; set; }

    public int? Quantity { get; set; }

    [StringLength(50)]
    public string? TransactionType { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("Transactions")]
    public virtual Product? Product { get; set; }
}
