# ?? EbikeRental Project Overview

> **E-Bike Manufacturing & Rental Management System**  
> Enterprise Resource Planning (ERP) System for E-Bike Production, Inventory, Quality Control, and Rental Operations

---

## ?? Table of Contents

1. [System Overview](#-system-overview)
2. [Technology Stack](#-technology-stack)
3. [Architecture](#-architecture)
4. [Module Details](#-module-details)
5. [Data Flow](#-data-flow)
6. [Implementation Status](#-implementation-status)
7. [Quick Start Guide](#-quick-start-guide)
8. [Development Roadmap](#-development-roadmap)

---

## ?? System Overview

### Purpose
EbikeRental is a comprehensive ERP system designed to manage the complete lifecycle of e-bike manufacturing and rental operations:

- **Manufacturing Management** - Production planning, BOM management, material issuing
- **Inventory Control** - Stock tracking, warehouse management, transactions
- **Purchasing** - PR/PO/GR workflow, vendor management
- **Quality Assurance** - Multi-stage QC (Incoming, In-Process, Final, Maintenance)
- **Rental Operations** - Contract management, asset tracking, returns processing
- **Maintenance** - Repair order management, service scheduling

### Key Features
? **Production Order Management** with BOM integration  
? **Material Requirements Planning (MRP)** from BOM  
? **Multi-warehouse Stock Management**  
? **Purchase Requisition ? Purchase Order ? Goods Receipt Workflow**  
? **Multi-stage Quality Control (QC) System**  
? **Asset Lifecycle Management**  
? **Rental Contract Management**  
? **Repair & Maintenance Tracking**  

---

## ?? Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **UI Pattern**: Razor Pages
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity

### Frontend
- **CSS Framework**: Tailwind CSS v3 (CDN)
- **Icons**: Bootstrap Icons
- **JavaScript**: Vanilla JS (No frameworks)

### Architecture Pattern
- **Clean Architecture** (Domain, Application, Infrastructure, Web)
- **Repository Pattern** with Generic Repository
- **Service Layer** for Business Logic
- **Result Pattern** for Error Handling
- **Dependency Injection** throughout

---

## ??? Architecture

### Project Structure

```
EbikeRental/
??? EbikeRental.Domain/              # Enterprise Entities & Enums
?   ??? Entities/                    # Domain Models (30+ entities)
?   ??? Enums/                       # Status enumerations
?
??? EbikeRental.Application/         # Business Logic Layer
?   ??? Services/                    # Business Services (12 services)
?   ??? DTOs/                        # Data Transfer Objects
?   ??? Interfaces/                  # Service & Repository Contracts
?
??? EbikeRental.Infrastructure/      # Data Access Layer
?   ??? Data/                        # DbContext & Configuration
?   ??? Repositories/                # Generic & Specific Repositories
?   ??? Migrations/                  # EF Core Migrations
?
??? EbikeRental.Shared/              # Cross-cutting Concerns
?   ??? Result.cs                    # Result Pattern Implementation
?
??? EbikeRental.Web/                 # Presentation Layer
    ??? Pages/                       # Razor Pages (organized by module)
    ?   ??? Masters/                 # Master Data (Items, BOM, Warehouses)
    ?   ??? Production/              # Production Orders, Material Issue
    ?   ??? Purchasing/              # PR, PO, GR
    ?   ??? Quality/                 # QC Management
    ?   ??? Inventory/               # Stock Management
    ?   ??? Rental/                  # Rental Contracts & Returns
    ?   ??? Maintenance/             # Repair Orders
    ??? wwwroot/                     # Static assets
```

### Key Design Patterns

#### 1. **Repository Pattern**
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### 2. **Result Pattern**
```csharp
public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public static Result Ok(string message = "Success") => new() { Success = true, Message = message };
    public static Result Fail(string message) => new() { Success = false, Message = message };
}
```

#### 3. **Service Layer**
```csharp
public class ProductionService : IProductionService
{
    private readonly IRepository<ProductionOrder> _repository;
    private readonly IBomService _bomService;
    // Business logic encapsulated in service
}
```

---

## ?? Module Details

### 1. ?? Production Module

#### **A. Production Orders**
**Purpose**: Manage manufacturing orders from planning to completion

**Features**:
- Create production orders with BOM selection
- Auto-generate material list from BOM
- Track production progress (quantity completed)
- Timeline tracking (Planned vs Actual dates)
- Status workflow: Draft ? Released ? InProgress ? Completed/Cancelled

**Files**:
- `/Pages/Production/ProductionOrders/Index.cshtml` - List view
- `/Pages/Production/ProductionOrders/Info.cshtml` - Create/Edit form (with BOM tabs)
- `/Pages/Production/ProductionOrders/Track.cshtml` - Progress tracking
- `/Application/Services/ProductionService.cs` - Business logic

**Entities**:
- `ProductionOrder` (Header)
- `ProductionOrderItem` (Line items - materials)
- Relationships: ? Item, ? BillOfMaterial, ? QualityCheck

**Business Rules**:
- Must select valid BOM before creating order
- Cannot update/delete Completed or Cancelled orders
- Cannot start production without posted Material Issue
- Material quantities calculated as: `BOM Quantity × Order Quantity`

#### **B. Material Issue**
**Purpose**: Track material withdrawal from warehouse to production floor

**Features**:
- Generate material list from Production Order
- Track required vs issued quantities
- Batch/Lot number tracking
- Auto-generate document numbers (MI-YYYY-####)
- Post to reduce inventory (TODO: Implementation pending)

**Files**:
- `/Pages/Production/MaterialIssue/Index.cshtml` - List view
- `/Pages/Production/MaterialIssue/Info.cshtml` - Create/Edit form
- `/Application/Services/MaterialIssueService.cs` - Business logic

**Entities**:
- `MaterialIssue` (Header)
- `MaterialIssueItem` (Line items)
- Relationships: ? ProductionOrder, ? Warehouse, ? Item

**Business Rules**:
- Status: Draft ? Posted
- Only Draft status can be edited/deleted
- Posting should reduce inventory (Implementation pending)
- Production Order can only start if MI is Posted

**Current Limitations**:
- ? Post does not reduce stock (TODO comment in service)
- ? No stock availability check
- ? No batch/lot tracking in inventory

---

### 2. ?? Purchasing Module

#### **A. Purchase Requisition (PR)**
**Purpose**: Request for materials/items to be purchased

**Features**:
- Create PR with multiple line items
- Approval workflow (Pending ? Approved/Rejected)
- Auto-generate PR numbers (PR-YYYY-####)
- Estimated price tracking

**Files**:
- `/Pages/Purchasing/PR/Index.cshtml`
- `/Pages/Purchasing/PR/Info.cshtml`
- `/Application/Services/PurchaseRequisitionService.cs`

**Entities**:
- `PurchaseRequisition` (Header)
- `PurchaseRequisitionItem` (Line items)

**Business Rules**:
- Status: Draft ? Pending ? Approved/Rejected
- Only Draft can be edited
- Only Draft can be deleted
- Approved PR can be converted to PO

#### **B. Purchase Order (PO)**
**Purpose**: Formal order sent to vendor

**Features**:
- Create PO manually or from approved PR
- Vendor selection
- Tax and discount calculation
- Delivery date tracking
- Auto-generate PO numbers (PO-YYYY-####)

**Files**:
- `/Pages/Purchasing/PO/Index.cshtml`
- `/Pages/Purchasing/PO/Info.cshtml`
- `/Application/Services/PurchaseOrderService.cs`

**Entities**:
- `PurchaseOrder` (Header)
- `PurchaseOrderItem` (Line items)
- Relationships: ? PurchaseRequisition, ? Vendor (via VendorName)

**Business Rules**:
- Status: Draft ? Confirmed ? Closed ? Cancelled
- Line total = `Quantity × Unit Price × (1 - Discount%) × (1 + Tax%)`
- Only Draft status can be edited
- Confirmed PO can be received via GR

#### **C. Goods Receipt (GR)**
**Purpose**: Record receipt of goods from vendor

**Features**:
- Create GR from PO or standalone
- QC acceptance (Accepted/Rejected per line)
- Batch/Lot tracking
- Expiry date tracking
- Auto-generate GR numbers (GR-YYYY-####)

**Files**:
- `/Pages/Purchasing/GR/Index.cshtml`
- `/Pages/Purchasing/GR/Info.cshtml`
- `/Application/Services/GoodsReceiptService.cs`

**Entities**:
- `GoodsReceipt` (Header)
- `GoodsReceiptItem` (Line items)
- Relationships: ? PurchaseOrder, ? Warehouse

**Business Rules**:
- Status: Draft ? Posted
- Only Draft can be edited/deleted
- Posting should increase inventory (Implementation pending)
- QC must pass before posting (Optional, not enforced yet)

**Current Limitations**:
- ? Post does not increase stock (TODO)
- ? QC integration not complete
- ? No partial receipt tracking

---

### 3. ? Quality Control Module

**Purpose**: Multi-stage quality inspection system

**QC Types**:
1. **Incoming** - Inspect goods received from vendors (GR)
2. **In-Process** - Inspect during production (Production Order)
3. **Final** - Inspect finished goods before delivery
4. **Maintenance** - Inspect after repair/maintenance

**Features**:
- Document-based QC with line items
- Status: Pending ? InProgress ? Passed/Failed/Conditional
- Reference to source documents (GR, PO, etc.)
- Inspector tracking
- Auto-generate QC numbers (QC-YYYY-####)

**Files**:
- `/Pages/Quality/QC/Index.cshtml`
- `/Pages/Quality/QC/Info.cshtml`
- `/Application/Services/QualityCheckService.cs`

**Entities**:
- `QualityCheck` (Header)
- `QualityCheckItem` (Line items - inspection points)
- Relationships: ? GoodsReceipt, ? ProductionOrder

**Business Rules**:
- Only Draft/InProgress can be edited
- Cannot delete Posted QC records
- Failed Incoming QC should block GR posting (Not enforced yet)
- Failed Final QC should block Production completion (Not enforced yet)

**Current Limitations**:
- ? QC results don't affect GR/Production workflow
- ? No QC templates/checklists
- ? No photo/attachment support

---

### 4. ?? Inventory Module

**Purpose**: Stock management and tracking

**Features Currently**:
- View stock levels
- Basic warehouse selection

**Files**:
- `/Pages/Inventory/Stock/Index.cshtml`
- `/Pages/Inventory/Stock/Index.cshtml.cs`

**Entities**:
- `Warehouse`
- (Inventory transactions not implemented yet)

**Current Limitations**:
- ? **No Stock Transaction logging** (Critical gap)
- ? No Stock Adjustment functionality
- ? No Stock Transfer between warehouses
- ? No Stock Count/Cycle Count
- ? No Inventory Valuation (FIFO/LIFO/Average)
- ? No Low Stock Alerts
- ? No Reorder Point management

**Required Implementation**:
```csharp
// TODO: Create InventoryTransaction entity
public class InventoryTransaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public string TransactionType { get; set; } // GoodsReceipt, MaterialIssue, Adjustment, Transfer
    public int? ReferenceId { get; set; } // GR Id, MI Id, etc.
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceQuantity { get; set; }
    public string? BatchNumber { get; set; }
}
```

---

### 5. ?? Rental Module

**Purpose**: Manage e-bike rental operations

#### **A. Rental Contracts**
**Features (Planned)**:
- Create rental agreements
- Customer information
- E-bike asset selection
- Rental period and pricing
- Deposit tracking

**Files**:
- `/Pages/Rental/Contracts/Index.cshtml`
- `/Pages/Rental/Contracts/Info.cshtml`

**Entities**:
- `RentalContract`
- Relationships: ? Asset, ? Customer (User)

**Status**: ?? **Frontend Only - Backend Not Implemented**

#### **B. Returns Processing**
**Features (Planned)**:
- Process e-bike returns
- Damage assessment
- Late fee calculation
- Deposit refund processing

**Files**:
- `/Pages/Rental/Returns/Index.cshtml`
- `/Pages/Rental/Returns/Process.cshtml`

**Status**: ?? **Frontend Only - Backend Not Implemented**

**Required Implementation**:
- RentalService with CRUD operations
- Asset availability checking
- Pricing calculation logic
- Payment integration
- Damage assessment workflow

---

### 6. ?? Maintenance Module

**Purpose**: Track repair and maintenance activities

**Features (Planned)**:
- Create repair orders
- Link to rental assets
- Track repair costs
- Technician assignment
- Spare parts usage

**Files**:
- `/Pages/Maintenance/RepairOrders/Index.cshtml`
- `/Pages/Maintenance/RepairOrders/Info.cshtml`

**Entities**:
- `RepairOrder`
- Relationships: ? Asset

**Status**: ?? **Frontend Only - Backend Not Implemented**

**Required Implementation**:
- RepairOrderService
- Maintenance scheduling logic
- Spare parts inventory integration
- Cost tracking and reporting
- Service history tracking

---

### 7. ?? Master Data

#### **A. Items**
**Purpose**: Product and material catalog

**Features**:
? CRUD operations
? Code, Name, Description
? Category grouping
? Unit of Measure
? Standard Cost
? Active/Inactive status

**Files**:
- `/Pages/Masters/Items/Index.cshtml`
- `/Pages/Masters/Items/Info.cshtml`
- `/Application/Services/ItemService.cs`

**Entity**: `Item`

#### **B. Bill of Materials (BOM)**
**Purpose**: Define product structure and manufacturing recipe

**Features**:
? Multi-level BOM support
? BOM Items (materials/components)
? BOM Processes (work instructions)
? BOM QC Steps (quality checkpoints)
? Quantity and UOM per component
? Scrap percentage
? Active/Inactive versions

**Files**:
- `/Pages/Masters/BOM/Index.cshtml`
- `/Pages/Masters/BOM/Info.cshtml`
- `/Application/Services/BomService.cs`

**Entities**:
- `BillOfMaterial` (Header)
- `BomItem` (Components)
- `BomProcess` (Manufacturing steps)
- `BomQc` (QC checkpoints)

#### **C. Warehouses**
**Purpose**: Define storage locations

**Features**:
? CRUD operations
? Code and Name
? Location details
? Active/Inactive status

**Files**:
- `/Pages/Masters/Warehouses/Index.cshtml`
- `/Pages/Masters/Warehouses/Info.cshtml`
- `/Application/Services/WarehouseService.cs`

**Entity**: `Warehouse`

#### **D. Assets**
**Purpose**: Track individual e-bikes and equipment

**Features**:
?? Basic CRUD (Incomplete)
- Asset Code
- Serial Number
- Link to Item
- Status (Available, Rented, Under Maintenance, Retired)
- Purchase date and cost

**Files**:
- `/Pages/Masters/Assets/Index.cshtml`
- `/Pages/Masters/Assets/Info.cshtml`

**Entity**: `Asset`
**Status**: ?? **Partially Implemented**

---

## ?? Data Flow

### Production Flow
```
???????????????????
?  Select Item    ?
?  (Finished      ?
?   Product)      ?
???????????????????
         ?
         v
???????????????????
?  Select BOM     ???? Auto-populate Materials
???????????????????     Processes, QC Steps
         ?
         v
???????????????????
? Create          ?
? Production      ?
? Order (Draft)   ?
???????????????????
         ?
         v
???????????????????
? Generate        ?
? Material Issue  ???? Deduct Stock (TODO)
? (Post)          ?
???????????????????
         ?
         v
???????????????????
? Start           ?
? Production      ?
? (InProgress)    ?
???????????????????
         ?
         v
???????????????????
? In-Process QC   ???? Optional checkpoints
???????????????????
         ?
         v
???????????????????
? Final QC        ???? Quality verification
???????????????????
         ?
         v
???????????????????
? Complete        ?
? Production      ???? Add Stock (TODO)
? Order           ?
???????????????????
```

### Purchasing Flow
```
???????????????????
? Create PR       ?
? (Draft)         ?
???????????????????
         ?
         v
???????????????????
? Approve PR      ?
?                 ?
???????????????????
         ?
         v
???????????????????
? Create PO       ???? From approved PR or standalone
? (Draft)         ?
???????????????????
         ?
         v
???????????????????
? Confirm PO      ???? Send to vendor
?                 ?
???????????????????
         ?
         v
???????????????????
? Receive Goods   ?
? Create GR       ?
? (Draft)         ?
???????????????????
         ?
         v
???????????????????
? Incoming QC     ???? Inspect received goods
???????????????????
         ?
         v
???????????????????
? Post GR         ???? Add Stock (TODO)
?                 ?
???????????????????
```

---

## ?? Implementation Status

### ? **Fully Implemented (Backend + Frontend)**
- ? Items Management
- ? BOM Management (with Items, Processes, QC)
- ? Warehouses Management
- ? Purchase Requisition (CRUD + Approval)
- ? Purchase Order (CRUD + from PR)
- ? Goods Receipt (CRUD + from PO)
- ? Production Order (CRUD + from BOM)
- ? Material Issue (CRUD + from PO)
- ? Quality Check (CRUD + Multi-type)

### ?? **Partially Implemented**
- ?? **Production Order** - Missing:
  - QC Integration (QCStatus column exists but not used)
  - CompletedQuantity tracking
  - GR generation on completion
  
- ?? **Material Issue** - Missing:
  - Post functionality (does not reduce stock)
  - Stock availability check
  - Batch/Lot tracking in inventory

- ?? **Goods Receipt** - Missing:
  - Post functionality (does not increase stock)
  - QC integration (doesn't block posting)
  - Partial receipt tracking

- ?? **Assets Management** - Missing:
  - Complete service layer
  - Rental integration

### ? **Not Implemented (Frontend Only)**
- ? **Inventory Transactions** (Critical Gap)
  - No transaction logging
  - No stock adjustment
  - No stock transfer
  - No stock count
  
- ? **Rental Module**
  - RentalService needed
  - Payment integration needed
  - Asset availability logic needed
  
- ? **Maintenance Module**
  - RepairOrderService needed
  - Spare parts integration needed
  - Cost tracking needed

---

## ?? Quick Start Guide

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 / VS Code

### Setup Steps

1. **Clone Repository**
```bash
git clone <repository-url>
cd EbikeRental
```

2. **Update Database Connection**
Edit `EbikeRental.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EbikeRentalDb;Trusted_Connection=true;"
  }
}
```

3. **Apply Migrations**
```bash
cd EbikeRental.Web
dotnet ef database update
```

4. **Run Application**
```bash
dotnet run
```

5. **Access Application**
```
https://localhost:5001
```

### Default Login (if Identity is configured)
- Username: `admin@ebikerental.com`
- Password: `Admin@123`

---

## ??? Development Roadmap

### **Phase 1: Critical Inventory Integration** (Priority 1)
**Goal**: Make stock transactions actually work

1. **Create InventoryTransaction Entity**
   - Transaction Type (GR, MI, Adjustment, Transfer)
   - Reference tracking (GR Id, MI Id, etc.)
   - Balance tracking

2. **Implement Inventory Service**
   - AddStock() method
   - DeductStock() method
   - AdjustStock() method
   - TransferStock() method
   - GetStockBalance() method

3. **Integrate with GR Post**
   - Call AddStock() when GR is Posted
   - Create inventory transaction records
   - Update stock balances

4. **Integrate with MI Post**
   - Call DeductStock() when MI is Posted
   - Check stock availability
   - Create inventory transaction records

5. **Integrate with Production Complete**
   - Call AddStock() for finished goods
   - Create GR automatically
   - Update inventory

**Estimated Effort**: 3-5 days

---

### **Phase 2: QC Integration** (Priority 2)
**Goal**: Make QC results affect workflow

1. **Link QC to GR**
   - Block GR Post if Incoming QC Failed
   - Auto-create QC record when GR is created

2. **Link QC to Production**
   - Update Production Order QCStatus
   - Block Production Complete if Final QC Failed
   - Track In-Process QC results

3. **QC Templates**
   - Create QC template entity
   - Pre-defined checkpoints
   - Reusable across items

**Estimated Effort**: 2-3 days

---

### **Phase 3: Rental Module Backend** (Priority 3)
**Goal**: Complete rental operations

1. **RentalService Implementation**
   - Create/Update/Delete contracts
   - Asset availability checking
   - Pricing calculation

2. **Returns Processing**
   - Damage assessment workflow
   - Late fee calculation
   - Deposit refund logic

3. **Payment Integration**
   - Payment recording
   - Receipt generation

**Estimated Effort**: 3-4 days

---

### **Phase 4: Maintenance Module Backend** (Priority 3)
**Goal**: Complete maintenance operations

1. **RepairOrderService Implementation**
   - Create/Update/Delete repair orders
   - Technician assignment
   - Status tracking

2. **Spare Parts Integration**
   - Link to inventory items
   - Track parts usage
   - Auto-deduct stock

3. **Cost Tracking**
   - Labor cost
   - Parts cost
   - Total repair cost

**Estimated Effort**: 2-3 days

---

### **Phase 5: Advanced Features** (Priority 4)
1. **Reporting Module**
   - Stock valuation report
   - Production efficiency report
   - QC summary report
   - Rental revenue report

2. **Dashboard**
   - KPIs (Stock level, Production status, QC pass rate)
   - Charts and graphs
   - Recent activities

3. **Advanced Inventory**
   - FIFO/LIFO costing
   - Reorder point alerts
   - ABC analysis

4. **Workflow Notifications**
   - Email alerts
   - Approval notifications
   - Low stock alerts

**Estimated Effort**: 5-7 days

---

## ?? Development Notes

### Naming Conventions
- **Entities**: PascalCase (e.g., `ProductionOrder`)
- **Properties**: PascalCase (e.g., `OrderNumber`)
- **Methods**: PascalCase (e.g., `GetByIdAsync`)
- **Variables**: camelCase (e.g., `productionOrder`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `MAX_QUANTITY`)

### Status Values (Standardized)
- **Draft** - Initial state, editable
- **Pending** - Awaiting approval
- **Approved** - Approved, ready for next step
- **Confirmed** - Confirmed/Released
- **InProgress** - Work in progress
- **Completed** - Successfully completed
- **Posted** - Financially posted (affects inventory)
- **Closed** - Closed, no further action
- **Cancelled** - Cancelled
- **Rejected** - Rejected, will not proceed

### Common Patterns

#### Controller Action (Razor Pages)
```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();

    var result = await _service.CreateAsync(Model, UserId);
    
    if (!result.Success)
    {
        TempData["ErrorMessage"] = result.Message;
        return Page();
    }

    TempData["SuccessMessage"] = result.Message;
    return RedirectToPage("./Index");
}
```

#### Service Method
```csharp
public async Task<Result<int>> CreateAsync(OrderDto dto, int userId)
{
    try
    {
        // Validation
        if (dto.Quantity <= 0)
            return Result<int>.Fail("Quantity must be greater than zero");

        // Business logic
        var entity = MapToEntity(dto);
        entity.CreatedByUserId = userId;
        entity.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(entity);

        return Result<int>.Ok(entity.Id, "Created successfully");
    }
    catch (Exception ex)
    {
        return Result<int>.Fail($"Error: {ex.Message}");
    }
}
```

---

## ?? Related Documentation

- `PURCHASING_CHECKLIST.md` - Purchasing module implementation details
- `README_PARTIALS.md` - Partial views usage guide
- `README_FILTERS.md` - Filtering and search implementation

---

## ?? Support & Contact

For questions or issues:
1. Check this documentation
2. Review service implementations
3. Examine existing working modules
4. Follow established patterns

---

**Last Updated**: 2025-01-03  
**Version**: 1.0  
**Project Status**: Active Development ??

---

## ?? Quick Stats

| Metric | Count |
|--------|-------|
| Entities | 30+ |
| Services | 12 |
| Razor Pages | 50+ |
| Modules | 7 |
| Database Tables | 30+ |
| Lines of Code | ~15,000 |

---

**Built with ?? using ASP.NET Core 9 & Tailwind CSS**
