using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBomReferenceToProductionOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillOfMaterialId",
                table: "ProductionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BomCode",
                table: "ProductionOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BomName",
                table: "ProductionOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductionOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    BomItemId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BomQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionOrderItems_BomItems_BomItemId",
                        column: x => x.BomItemId,
                        principalTable: "BomItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrderItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrderItems_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_BillOfMaterialId",
                table: "ProductionOrders",
                column: "BillOfMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderItems_BomItemId",
                table: "ProductionOrderItems",
                column: "BomItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderItems_ItemId",
                table: "ProductionOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderItems_ProductionOrderId_Sequence",
                table: "ProductionOrderItems",
                columns: new[] { "ProductionOrderId", "Sequence" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionOrders_BillOfMaterials_BillOfMaterialId",
                table: "ProductionOrders",
                column: "BillOfMaterialId",
                principalTable: "BillOfMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionOrders_BillOfMaterials_BillOfMaterialId",
                table: "ProductionOrders");

            migrationBuilder.DropTable(
                name: "ProductionOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductionOrders_BillOfMaterialId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "BillOfMaterialId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "BomCode",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "BomName",
                table: "ProductionOrders");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductionOrders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "QCStatus",
                table: "ProductionOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ProductionOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
