using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeSerialTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBarcode",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSerial",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "GoodsReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "GoodsReceiptItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBarcode",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsSerial",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "GoodsReceiptItems");
        }
    }
}
