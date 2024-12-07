using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncSupplierIdIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.RenameColumn(
            //     name: "ContactInfo",
            //     table: "Suppliers",
            //     newName: "PhoneNumber");
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "EmailAddress",
            //     table: "Suppliers",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "EmailAddress",
            //     table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Suppliers",
                newName: "ContactInfo");
        }
    }
}
