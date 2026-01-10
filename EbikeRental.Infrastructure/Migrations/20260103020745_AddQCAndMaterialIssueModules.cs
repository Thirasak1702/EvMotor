using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQCAndMaterialIssueModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectedQuantity",
                table: "ProductionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QCStatus",
                table: "GoodsReceiptItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QualityCheckId",
                table: "GoodsReceiptItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresQC",
                table: "GoodsReceiptItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BillOfMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BomCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentItemId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillOfMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillOfMaterials_Items_ParentItemId",
                        column: x => x.ParentItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialIssues_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialIssues_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CheckType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GoodsReceiptId = table.Column<int>(type: "int", nullable: true),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: true),
                    DefectDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityChecks_GoodsReceipts_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "GoodsReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityChecks_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BomItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillOfMaterialId = table.Column<int>(type: "int", nullable: false),
                    ComponentItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScrapPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BomItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BomItems_BillOfMaterials_BillOfMaterialId",
                        column: x => x.BillOfMaterialId,
                        principalTable: "BillOfMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BomItems_Items_ComponentItemId",
                        column: x => x.ComponentItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialIssueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialIssueId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialIssueItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialIssueItems_MaterialIssues_MaterialIssueId",
                        column: x => x.MaterialIssueId,
                        principalTable: "MaterialIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualityCheckId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    InspectedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ItemStatus = table.Column<int>(type: "int", nullable: false),
                    DefectDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityCheckItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityCheckItems_QualityChecks_QualityCheckId",
                        column: x => x.QualityCheckId,
                        principalTable: "QualityChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterials_BomCode",
                table: "BillOfMaterials",
                column: "BomCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterials_ParentItemId",
                table: "BillOfMaterials",
                column: "ParentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_BillOfMaterialId",
                table: "BomItems",
                column: "BillOfMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ComponentItemId",
                table: "BomItems",
                column: "ComponentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueItems_ItemId",
                table: "MaterialIssueItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueItems_MaterialIssueId",
                table: "MaterialIssueItems",
                column: "MaterialIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssues_ProductionOrderId",
                table: "MaterialIssues",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssues_WarehouseId",
                table: "MaterialIssues",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCheckItems_ItemId",
                table: "QualityCheckItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCheckItems_QualityCheckId",
                table: "QualityCheckItems",
                column: "QualityCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_GoodsReceiptId",
                table: "QualityChecks",
                column: "GoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_ProductionOrderId",
                table: "QualityChecks",
                column: "ProductionOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BomItems");

            migrationBuilder.DropTable(
                name: "MaterialIssueItems");

            migrationBuilder.DropTable(
                name: "QualityCheckItems");

            migrationBuilder.DropTable(
                name: "BillOfMaterials");

            migrationBuilder.DropTable(
                name: "MaterialIssues");

            migrationBuilder.DropTable(
                name: "QualityChecks");

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

            migrationBuilder.DropColumn(
                name: "QCStatus",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "QualityCheckId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "RequiresQC",
                table: "GoodsReceiptItems");
        }
    }
}
