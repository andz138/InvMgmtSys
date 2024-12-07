﻿// <auto-generated />
using System;
using InventoryManagementSystem.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    [DbContext(typeof(InventoryMgmtDbContext))]
    [Migration("20241104221734_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InventoryManagementSystem.Models.Inventory", b =>
                {
                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastRestockDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("StockLevel")
                        .HasColumnType("int");

                    b.Property<int?>("SupplierID")
                        .HasColumnType("int");

                    b.HasKey("ProductID")
                        .HasName("PK__Inventor__B40CC6EDF7E4399A");

                    b.HasIndex("SupplierID");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("ReorderThreshold")
                        .HasColumnType("int");

                    b.Property<string>("SKU")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int?>("SupplierID")
                        .HasColumnType("int");

                    b.HasKey("ProductID")
                        .HasName("PK__Products__B40CC6ED91555A8C");

                    b.HasIndex("SupplierID");

                    b.HasIndex(new[] { "SKU" }, "UQ__Products__CA1ECF0D75A1D6CD")
                        .IsUnique()
                        .HasFilter("[SKU] IS NOT NULL");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Supplier", b =>
                {
                    b.Property<int>("SupplierID")
                        .HasColumnType("int");

                    b.Property<string>("ContactInfo")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Location")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SupplierName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SupplierID")
                        .HasName("PK__Supplier__4BE66694A43B676C");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionID")
                        .HasColumnType("int");

                    b.Property<int?>("ProductID")
                        .HasColumnType("int");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TransactionDate")
                        .HasColumnType("datetime");

                    b.Property<string>("TransactionType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TransactionID")
                        .HasName("PK__Transact__55433A4BDFC87929");

                    b.HasIndex("ProductID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Inventory", b =>
                {
                    b.HasOne("InventoryManagementSystem.Models.Product", "Product")
                        .WithOne("Inventory")
                        .HasForeignKey("InventoryManagementSystem.Models.Inventory", "ProductID")
                        .IsRequired()
                        .HasConstraintName("FK__Inventory__Produ__3D5E1FD2");

                    b.HasOne("InventoryManagementSystem.Models.Supplier", "Supplier")
                        .WithMany("Inventories")
                        .HasForeignKey("SupplierID")
                        .HasConstraintName("FK__Inventory__Suppl__3E52440B");

                    b.Navigation("Product");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Product", b =>
                {
                    b.HasOne("InventoryManagementSystem.Models.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierID")
                        .HasConstraintName("FK__Products__Suppli__3A81B327");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Transaction", b =>
                {
                    b.HasOne("InventoryManagementSystem.Models.Product", "Product")
                        .WithMany("Transactions")
                        .HasForeignKey("ProductID")
                        .HasConstraintName("FK__Transacti__Produ__412EB0B6");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Product", b =>
                {
                    b.Navigation("Inventory");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Supplier", b =>
                {
                    b.Navigation("Inventories");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
