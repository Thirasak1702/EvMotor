using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbikeRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionProcessesAndQcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionOrderProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    BomProcessId = table.Column<int>(type: "int", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    WorkCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumberOfPersons = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionOrderProcesses_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrderQcs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    BomQcId = table.Column<int>(type: "int", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    QcCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QcName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QcValues = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedByUserId = table.Column<int>(type: "int", nullable: true),
                    ActualValues = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderQcs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionOrderQcs_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderProcesses_ProductionOrderId_Sequence",
                table: "ProductionOrderProcesses",
                columns: new[] { "ProductionOrderId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderQcs_ProductionOrderId_Sequence",
                table: "ProductionOrderQcs",
                columns: new[] { "ProductionOrderId", "Sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionOrderProcesses");

            migrationBuilder.DropTable(
                name: "ProductionOrderQcs");
        }
    }
}
