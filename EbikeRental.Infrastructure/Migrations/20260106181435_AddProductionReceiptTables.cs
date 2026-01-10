using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionReceiptTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK/Index/Column only if they exist (avoid failure when DB already removed the column)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterialIssueItems_ProductionOrderItems_ProductionOrderItemId')
    ALTER TABLE MaterialIssueItems DROP CONSTRAINT FK_MaterialIssueItems_ProductionOrderItems_ProductionOrderItemId;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MaterialIssueItems_ProductionOrderItemId')
    DROP INDEX IX_MaterialIssueItems_ProductionOrderItemId ON MaterialIssueItems;
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'ProductionOrderItemId' AND Object_ID = Object_ID('MaterialIssueItems'))
    ALTER TABLE MaterialIssueItems DROP COLUMN ProductionOrderItemId;
");

            migrationBuilder.CreateTable(
                name: "ProductionReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReceipts_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionReceipts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReceiptItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReceiptId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReceiptItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionReceiptItems_ProductionReceipts_ProductionReceiptId",
                        column: x => x.ProductionReceiptId,
                        principalTable: "ProductionReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReceiptItems_ItemId",
                table: "ProductionReceiptItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReceiptItems_ProductionReceiptId",
                table: "ProductionReceiptItems",
                column: "ProductionReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReceipts_DocumentNumber",
                table: "ProductionReceipts",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReceipts_ProductionOrderId",
                table: "ProductionReceipts",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReceipts_WarehouseId",
                table: "ProductionReceipts",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionReceiptItems");

            migrationBuilder.DropTable(
                name: "ProductionReceipts");

            migrationBuilder.AddColumn<int>(
                name: "ProductionOrderItemId",
                table: "MaterialIssueItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueItems_ProductionOrderItemId",
                table: "MaterialIssueItems",
                column: "ProductionOrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialIssueItems_ProductionOrderItems_ProductionOrderItemId",
                table: "MaterialIssueItems",
                column: "ProductionOrderItemId",
                principalTable: "ProductionOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
