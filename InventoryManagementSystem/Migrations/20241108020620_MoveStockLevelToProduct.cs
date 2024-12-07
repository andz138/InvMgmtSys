using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class MoveStockLevelToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockLevel",
                table: "Inventory");

            // migrationBuilder.AlterColumn<int>(
            //     name: "SupplierID",
            //     table: "Suppliers",
            //     type: "int",
            //     nullable: false,
            //     oldClrType: typeof(int),
            //     oldType: "int")
            //     .Annotation("SqlServer:Identity", "1, 1");
            //
            // migrationBuilder.AlterColumn<int>(
            //     name: "ProductID",
            //     table: "Products",
            //     type: "int",
            //     nullable: false,
            //     oldClrType: typeof(int),
            //     oldType: "int")
            //     .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "StockLevel",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockLevel",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierID",
                table: "Suppliers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "StockLevel",
                table: "Inventory",
                type: "int",
                nullable: true);
        }
    }
}
