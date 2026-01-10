using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductionOrderSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedQuantity",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "FinalQCId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "InProcessQCId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "QCStatus",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "RejectedQuantity",
                table: "ProductionOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedQuantity",
                table: "ProductionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinalQCId",
                table: "ProductionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InProcessQCId",
                table: "ProductionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QCStatus",
                table: "ProductionOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectedQuantity",
                table: "ProductionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
