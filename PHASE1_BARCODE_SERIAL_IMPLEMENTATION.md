# Phase 1: Barcode & Serial Number Implementation for Goods Receipt

> **วัตถุประสงค์**: เพิ่มการเก็บ Barcode และ Serial Number ในขั้นตอนรับของเข้า Stock (Goods Receipt)  
> **ขอบเขต**: เริ่มที่ GR ก่อน เพื่อใช้ตอนรับของเข้าและเตรียมสำหรับ Material Issue ต่อไป  
> **วันที่จัดทำ**: 5 มกราคม 2026

---

## 📋 สรุปความต้องการ

### ✅ ข้อกำหนดหลัก
1. **Barcode/Serial ต่อ Line**: เก็บหนึ่งค่าต่อหนึ่งแถว GR (ไม่ต้องรองรับหลายค่า)
2. **Flag ต่อสินค้า**: มี `IsBarcode` และ `IsSerial` ใน Item เพื่อควบคุมการแสดง/บังคับ
3. **Validation ที่ GR**: เก็บเป็นข้อมูลอ้างอิง ไม่ต้องเช็ค uniqueness ในสโคปนี้
4. **Flow**: GR line → Post GR → InventoryTransaction (พร้อม Barcode/Serial)

### 📦 สินค้าที่ได้รับผลกระทบ
- สินค้าที่ต้อง track Barcode: ตั้ง `IsBarcode = true` ใน Item
- สินค้าที่ต้อง track Serial: ตั้ง `IsSerial = true` ใน Item
- สินค้าปกติ: ทั้งสอง flag เป็น `false` (ไม่แสดงฟิลด์)

---

## 🗂️ โครงสร้างข้อมูล (Schema Changes)

### 1️⃣ ตาราง `Items` (Master Data)

**เพิ่มคอลัมน์**:ALTER TABLE Items ADD IsBarcode BIT NOT NULL DEFAULT 0;
ALTER TABLE Items ADD IsSerial BIT NOT NULL DEFAULT 0;
ALTER TABLE Items ADD Barcode NVARCHAR(MAX) NULL;
ALTER TABLE Items ADD TrackingMethod NVARCHAR(MAX) NOT NULL DEFAULT N'';**คำอธิบาย**:
- `IsBarcode`: สินค้าต้องการ Barcode tracking (true/false)
- `IsSerial`: สินค้าต้องการ Serial Number tracking (true/false)
- `Barcode`: เก็บ Barcode ของสินค้า (nullable)
- `TrackingMethod`: ระบุวิธีการติดตามสินค้า (`None`, `Batch`, `Serial`, `Both`)
- Default: `false` (ไม่บังคับ) เพื่อไม่กระทบข้อมูลเดิม

**Entity Class** (`EbikeRental.Domain/Entities/Item.cs`):public class Item
{
    // ... existing properties ...
    
    public bool IsBarcode { get; set; }
    public bool IsSerial { get; set; }
    public string? Barcode { get; set; }
    public string TrackingMethod { get; set; } = "None";
}---

### 2️⃣ ตาราง `GoodsReceiptItems` (GR Line)

**เพิ่มคอลัมน์**:ALTER TABLE GoodsReceiptItems ADD Barcode NVARCHAR(100) NULL;
ALTER TABLE GoodsReceiptItems ADD SerialNumber NVARCHAR(100) NULL;**คำอธิบาย**:
- `Barcode`: เก็บ Barcode ที่รับเข้ามา (nullable)
- `SerialNumber`: เก็บ Serial Number ที่รับเข้ามา (nullable)
- Nullable เพราะไม่ใช่ทุกสินค้าต้องมี

**Entity Class** (`EbikeRental.Domain/Entities/GoodsReceiptItem.cs`):public class GoodsReceiptItem
{
    // ... existing properties ...
    
    public string? Barcode { get; set; }
    public string? SerialNumber { get; set; }
}---

### 3️⃣ ตาราง `InventoryTransactions` (New Table - ตาม Phase 1 Core)

**สร้างตารางใหม่** (ถ้ายังไม่มี):CREATE TABLE InventoryTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransactionDate DATETIME2 NOT NULL,
    ItemId INT NOT NULL,
    WarehouseId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL, -- 'GoodsReceipt', 'MaterialIssue', 'Adjustment', 'Transfer'
    ReferenceId INT NULL,                  -- GR Id, MI Id, etc.
    ReferenceLineId INT NULL,              -- GR Line Id, MI Line Id (ใหม่เพื่อ trace line-level)
    Quantity DECIMAL(18,3) NOT NULL,
    UnitCost DECIMAL(18,2) NULL,
    BalanceQuantity DECIMAL(18,3) NOT NULL,
    BatchNumber NVARCHAR(50) NULL,
    Barcode NVARCHAR(100) NULL,            -- ใหม่: มาจาก GR line
    SerialNumber NVARCHAR(100) NULL,       -- ใหม่: มาจาก GR line
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    
    CONSTRAINT FK_InventoryTransactions_Items FOREIGN KEY (ItemId) REFERENCES Items(Id),
    CONSTRAINT FK_InventoryTransactions_Warehouses FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id)
);**Entity Class** (`EbikeRental.Domain/Entities/InventoryTransaction.cs`):public class InventoryTransaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public int? ReferenceLineId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal BalanceQuantity { get; set; }
    public string? BatchNumber { get; set; }
    public string? Barcode { get; set; }
    public string? SerialNumber { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Item Item { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}---

## 🔄 ขั้นตอนการพัฒนา

### **Step 1: Database Migration**

**ไฟล์ที่ต้องสร้าง**:
- `EbikeRental.Infrastructure/Migrations/YYYYMMDDHHMMSS_AddBarcodeSerialTracking.cs`

**คำสั่ง Migration**:# ใน EbikeRental.Web directory
dotnet ef migrations add AddBarcodeSerialTracking --project ../EbikeRental.Infrastructure

# Review migration file ก่อน apply

dotnet ef database update**เนื้อหา Migration**:public partial class AddBarcodeSerialTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. เพิ่ม flag และ Barcode/TrackingMethod ใน Items
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
            table: "Items",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TrackingMethod",
            table: "Items",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        // 2. เพิ่ม Barcode/Serial ใน GoodsReceiptItems
        migrationBuilder.AddColumn<string>(
            name: "Barcode",
            table: "GoodsReceiptItems",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "SerialNumber",
            table: "GoodsReceiptItems",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        // 3. สร้างตาราง InventoryTransactions (ถ้ายังไม่มี)
        migrationBuilder.CreateTable(
            name: "InventoryTransactions",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ItemId = table.Column<int>(nullable: false),
                WarehouseId = table.Column<int>(nullable: false),
                TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ReferenceId = table.Column<int>(nullable: true),
                ReferenceLineId = table.Column<int>(nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                BalanceQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CreatedByUserId = table.Column<int>(nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_InventoryTransactions_Items_ItemId",
                    column: x => x.ItemId,
                    principalTable: "Items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_InventoryTransactions_Warehouses_WarehouseId",
                    column: x => x.WarehouseId,
                    principalTable: "Warehouses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_ItemId",
            table: "InventoryTransactions",
            column: "ItemId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_WarehouseId",
            table: "InventoryTransactions",
            column: "WarehouseId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_TransactionDate",
            table: "InventoryTransactions",
            column: "TransactionDate");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "InventoryTransactions");
        migrationBuilder.DropColumn(name: "IsBarcode", table: "Items");
        migrationBuilder.DropColumn(name: "IsSerial", table: "Items");
        migrationBuilder.DropColumn(name: "Barcode", table: "Items");
        migrationBuilder.DropColumn(name: "TrackingMethod", table: "Items");
        migrationBuilder.DropColumn(name: "Barcode", table: "GoodsReceiptItems");
        migrationBuilder.DropColumn(name: "SerialNumber", table: "GoodsReceiptItems");
    }
}---

## ✅ Validation Rules & Business Logic

### 1️⃣ Item Master Data
- ✅ Flag `IsBarcode` และ `IsSerial` เป็น optional (default false)
- ✅ สามารถเปิดทั้งสองอย่างพร้อมกันได้
- ✅ `TrackingMethod` ระบุวิธีการติดตามสินค้า

### 2️⃣ GR Line Entry
- ✅ ถ้า `IsBarcode = true` ต้องกรอก Barcode ก่อน Save (Client + Server validation)
- ✅ ถ้า `IsSerial = true` ต้องกรอก Serial Number ก่อน Save (Client + Server validation)
- ✅ ถ้าทั้งสอง flag เป็น `false` ไม่แสดงฟิลด์ (แสดง "N/A")
- ✅ ยังไม่ validate uniqueness (เก็บเป็นข้อมูลอ้างอิงเท่านั้น)

### 3️⃣ GR Post
- ✅ ตรวจสอบ required fields ก่อน Post
- ✅ บันทึก Barcode/Serial ลง InventoryTransaction ทุกแถว
- ✅ ถ้าสต๊อกบันทึกไม่ได้ rollback และแจ้ง error

### 4️⃣ Inventory Transaction
- ✅ เก็บ ReferenceLineId เพื่อ trace กลับไป GR line ได้
- ✅ Barcode/Serial เป็น nullable (เพราะไม่ใช่ทุกสินค้ามี)

---

## 🎯 Summary

การเพิ่ม Barcode/Serial Tracking ใน Phase 1 จะทำให้ระบบ:
1. ✅ ติดตามสินค้าได้ละเอียดขึ้น (line-level tracking)
2. ✅ รองรับการ scan barcode ตอนรับ/เบิกของ
3. ✅ เก็บประวัติ transaction พร้อม reference ย้อนกลับได้
4. ✅ พร้อมขยายไป MI, Production, และ Stock Count ต่อได้

**ขอบเขตที่ครอบคลุม Phase 1**:
- ✅ GR รับของเข้า (มี Barcode/Serial)
- ⏳ MI เบิกของออก (ขั้นต่อไป)
- ⏳ Production Complete (ขั้นต่อไป)
- ⏳ Stock Balance Inquiry (ขั้นต่อไป)

**โฟลว์ครอบคลุมแล้ว**: 60% (เฉพาะ GR)  
**โฟลว์ยังขาด**: 40% (MI + Production + Adjustment)

---

**Updated:** 2026-01-06  
**Status:** ✅ **COMPLETE**  
**Ready for Testing:** ✅ **YES**  
**Ready for Production:** ⚠️ **AFTER TESTING**

