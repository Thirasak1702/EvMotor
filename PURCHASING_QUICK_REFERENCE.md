# Purchasing Module - Quick Reference Guide

## How to Use the Services in Razor Pages

### Example: Purchase Requisition Page Model

```csharp
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Purchasing.PurchaseRequisitions
{
    public class IndexModel : PageModel
    {
        private readonly IPurchaseRequisitionService _prService;

        public IndexModel(IPurchaseRequisitionService prService)
        {
            _prService = prService;
        }

        public List<PurchaseRequisitionDto> PurchaseRequisitions { get; set; }

        public async Task OnGetAsync()
        {
            var result = await _prService.GetAllAsync();
            if (result.IsSuccess)
            {
                PurchaseRequisitions = result.Data;
            }
        }
    }
}
```

### Example: Create Purchase Requisition

```csharp
public class CreateModel : PageModel
{
    private readonly IPurchaseRequisitionService _prService;
    private readonly IItemService _itemService;

    [BindProperty]
    public PurchaseRequisitionDto PurchaseRequisition { get; set; }

    public CreateModel(
        IPurchaseRequisitionService prService,
        IItemService itemService)
    {
        _prService = prService;
        _itemService = itemService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = 1; // Get from User.Claims
        var result = await _prService.CreateAsync(PurchaseRequisition, userId);
        
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToPage("./Index");
        }
        
        TempData["ErrorMessage"] = result.ErrorMessage;
        return Page();
    }
}
```

### Example: Approve Purchase Requisition

```csharp
public async Task<IActionResult> OnPostApproveAsync(int id)
{
    var userId = 1; // Get from User.Claims
    var result = await _prService.ApproveAsync(id, userId);
    
    if (result.IsSuccess)
    {
        TempData["SuccessMessage"] = "Purchase requisition approved successfully";
    }
    else
    {
        TempData["ErrorMessage"] = result.ErrorMessage;
    }
    
    return RedirectToPage("./Index");
}
```

---

## API Endpoints (if needed)

### Purchase Requisition Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class PurchaseRequisitionsController : ControllerBase
{
    private readonly IPurchaseRequisitionService _prService;

    public PurchaseRequisitionsController(IPurchaseRequisitionService prService)
    {
        _prService = prService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _prService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _prService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);
        
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseRequisitionDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _prService.CreateAsync(dto, userId);
        
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PurchaseRequisitionDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _prService.UpdateAsync(id, dto, userId);
        
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return Ok(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _prService.DeleteAsync(id);
        
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return Ok(result.Message);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _prService.ApproveAsync(id, userId);
        
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return Ok(result.Message);
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] string reason)
    {
        var userId = GetCurrentUserId();
        var result = await _prService.RejectAsync(id, userId, reason);
        
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        
        return Ok(result.Message);
    }

    private int GetCurrentUserId()
    {
        // Extract user ID from JWT token or Claims
        return int.Parse(User.FindFirst("UserId")?.Value ?? "0");
    }
}
```

---

## Sample DTO Creation

### Creating a Purchase Requisition with Items

```csharp
var pr = new PurchaseRequisitionDto
{
    Date = DateTime.Now,
    DepartmentName = "IT Department",
    RequestorName = "John Doe",
    Notes = "Urgent requirement for new laptops",
    Items = new List<PurchaseRequisitionItemDto>
    {
        new PurchaseRequisitionItemDto
        {
            ItemId = 1,
            Description = "Dell Laptop Model X",
            Quantity = 5,
            UnitOfMeasure = "PCS",
            EstimatedUnitPrice = 25000,
            RequiredDate = DateTime.Now.AddDays(7),
            Notes = "Latest model required"
        },
        new PurchaseRequisitionItemDto
        {
            ItemId = 2,
            Description = "Laptop Bag",
            Quantity = 5,
            UnitOfMeasure = "PCS",
            EstimatedUnitPrice = 500,
            RequiredDate = DateTime.Now.AddDays(7)
        }
    }
};

var result = await _prService.CreateAsync(pr, userId);
```

### Creating a Purchase Order

```csharp
var po = new PurchaseOrderDto
{
    OrderDate = DateTime.Now,
    VendorName = "ABC Supplies Co.",
    VendorContact = "vendor@abc.com",
    DeliveryAddress = "123 Main Street, Bangkok",
    ExpectedDeliveryDate = DateTime.Now.AddDays(14),
    Notes = "Please deliver during business hours",
    Items = new List<PurchaseOrderItemDto>
    {
        new PurchaseOrderItemDto
        {
            ItemId = 1,
            Description = "Dell Laptop",
            Quantity = 5,
            UnitOfMeasure = "PCS",
            UnitPrice = 24000,
            DiscountPercent = 5,
            TaxPercent = 7,
            Notes = "Include warranty"
        }
    }
};

var result = await _poService.CreateAsync(po, userId);
```

### Creating a Goods Receipt

```csharp
var gr = new GoodsReceiptDto
{
    ReceiptDate = DateTime.Now,
    PurchaseOrderId = 1, // Optional: link to PO
    VendorName = "ABC Supplies Co.",
    ReceivedBy = "Jane Smith",
    WarehouseId = 1,
    Notes = "All items received in good condition",
    Items = new List<GoodsReceiptItemDto>
    {
        new GoodsReceiptItemDto
        {
            ItemId = 1,
            OrderedQuantity = 5,
            ReceivedQuantity = 5,
            UnitOfMeasure = "PCS",
            BatchNumber = "BATCH-2024-001",
            IsAccepted = true,
            Notes = "Quality checked"
        }
    }
};

var result = await _grService.CreateAsync(gr, userId);
```

---

## Status Management

### Purchase Requisition Status Flow
```
Draft ? (Submit) ? Pending ? (Approve/Reject) ? Approved/Rejected
```

### Purchase Order Status Flow
```
Draft ? (Send) ? Sent ? (Confirm) ? Confirmed ? 
    ? (Receive) ? PartialReceived ? (Complete) ? Received
    
Can Cancel at any stage except Received
```

### Goods Receipt Status Flow
```
Draft ? (Post) ? Posted

Can Cancel at any stage
```

---

## Error Handling

All service methods return a `Result<T>` object with:
- `IsSuccess`: bool indicating success/failure
- `Data`: The returned data (for successful operations)
- `Message`: Success message
- `ErrorMessage`: Error description (for failed operations)

Always check `IsSuccess` before accessing `Data`:

```csharp
var result = await _prService.GetByIdAsync(id);
if (result.IsSuccess)
{
    var pr = result.Data;
    // Use pr...
}
else
{
    // Handle error
    TempData["ErrorMessage"] = result.ErrorMessage;
}
```

---

## Common Validation Rules

1. **Document Number**: Auto-generated, cannot be modified
2. **Status**: Only Draft records can be edited or deleted
3. **Items**: Must have at least one item
4. **Item References**: ItemId must exist in Items table
5. **Warehouse References**: WarehouseId must exist in Warehouses table
6. **Quantities**: Must be greater than 0
7. **Prices**: Must be non-negative

---

## Tips for Frontend Development

1. **Use Status Badges**: Show colored badges based on status
   - Draft: Gray
   - Pending: Orange
   - Approved: Green
   - Rejected: Red
   - Cancelled: Dark Red

2. **Conditional Actions**: Show/hide buttons based on status
   - Edit/Delete: Only for Draft status
   - Approve/Reject: Only for Pending status
   - Confirm: Only for Sent status

3. **Item Selection**: Use autocomplete or dropdown for item selection

4. **Calculations**: Calculate totals on the client side for immediate feedback

5. **Date Pickers**: Use date pickers for all date fields

6. **Validation**: Add client-side validation for better UX

---

## Database Queries for Reporting

### Get all Purchase Requisitions with total amounts
```sql
SELECT 
    pr.DocumentNumber,
    pr.Date,
    pr.DepartmentName,
    pr.Status,
    SUM(pri.Quantity * pri.EstimatedUnitPrice) as TotalAmount
FROM PurchaseRequisitions pr
INNER JOIN PurchaseRequisitionItems pri ON pr.Id = pri.PurchaseRequisitionId
GROUP BY pr.Id, pr.DocumentNumber, pr.Date, pr.DepartmentName, pr.Status
ORDER BY pr.Date DESC
```

### Get pending Purchase Requisitions
```sql
SELECT * FROM PurchaseRequisitions 
WHERE Status = 'Pending'
ORDER BY Date ASC
```

### Get Goods Receipts by date range
```sql
SELECT gr.*, w.Name as WarehouseName
FROM GoodsReceipts gr
INNER JOIN Warehouses w ON gr.WarehouseId = w.Id
WHERE gr.ReceiptDate BETWEEN @StartDate AND @EndDate
ORDER BY gr.ReceiptDate DESC
```
