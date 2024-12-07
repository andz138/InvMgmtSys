using ModelTrainer.Models;
using Microsoft.EntityFrameworkCore;

namespace ModelTrainer.Context;

public partial class InventoryMgmtDbContext : DbContext
{
    public InventoryMgmtDbContext(DbContextOptions<InventoryMgmtDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<SalesData> SalesData { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryEntryID).HasName("PK__Inventor__B40CC6EDF7E4399A");

            entity.Property(e => e.InventoryEntryID).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Product).WithOne(p => p.Inventory)
                .HasForeignKey<Inventory>(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__3D5E1FD2");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Inventories).HasConstraintName("FK__Inventory__Suppl__3E52440B");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID).HasName("PK__Products__B40CC6ED91555A8C");

            entity.Property(e => e.ProductID).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasConstraintName("FK__Products__Suppli__3A81B327");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierID).HasName("PK__Supplier__4BE66694A43B676C");

            entity.Property(e => e.SupplierID).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionID).HasName("PK__Transact__55433A4BDFC87929");

            entity.Property(e => e.TransactionID).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Product).WithMany(p => p.Transactions).HasConstraintName("FK__Transacti__Produ__412EB0B6");
        });
    }

}
