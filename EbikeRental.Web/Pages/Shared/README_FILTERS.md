# Filter Card Usage Guide

## Overview
This guide shows how to add consistent filter functionality to Index/List pages.

---

## Method 1: Manual Filter (Current Implementation)

### In PageModel (.cshtml.cs)
```csharp
[BindProperty(SupportsGet = true)]
public string? Code { get; set; }

[BindProperty(SupportsGet = true)]
public string? Name { get; set; }

[BindProperty(SupportsGet = true)]
public bool? IsActive { get; set; }

public async Task OnGetAsync()
{
    var result = await _service.GetAllAsync();
    if (result.Success)
    {
        Items = result.Data;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(Code))
        {
            Items = Items.Where(i => i.Code.Contains(Code, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            Items = Items.Where(i => i.Name.Contains(Name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (IsActive.HasValue)
        {
            Items = Items.Where(i => i.IsActive == IsActive.Value).ToList();
        }
    }
}
```

### In View (.cshtml)
```razor
<div class="card shadow-sm mb-3">
    <div class="card-header bg-light">
        <h5 class="mb-0"><i class="bi bi-funnel me-2"></i>Filters</h5>
    </div>
    <div class="card-body">
        <form method="get">
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">Code</label>
                    <input type="text" class="form-control" name="code" value="@Model.Code" placeholder="Search by code..." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Name</label>
                    <input type="text" class="form-control" name="name" value="@Model.Name" placeholder="Search by name..." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Status</label>
                    <select class="form-select" name="isActive">
                        <option value="">All</option>
                        <option value="true" selected="@(Model.IsActive == true)">Active</option>
                        <option value="false" selected="@(Model.IsActive == false)">Inactive</option>
                    </select>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <button type="submit" class="btn btn-primary">
                        <i class="bi bi-search"></i> Search
                    </button>
                    <a asp-page="Index" class="btn btn-secondary">
                        <i class="bi bi-x-circle"></i> Clear
                    </a>
                </div>
            </div>
        </form>
    </div>
</div>
```

---

## Method 2: Using _FilterCard Partial (Advanced)

### Setup ViewData in View
```razor
@{
    var filterFields = new List<(string Label, string Name, string Type, string Value, object Options)>
    {
        ("Code", "code", "text", Model.Code, null),
        ("Name", "name", "text", Model.Name, null),
        ("Category", "category", "text", Model.Category, null),
        ("Status", "isActive", "select", Model.IsActive?.ToString(), 
            new[] { ("", "All"), ("true", "Active"), ("false", "Inactive") })
    };
    ViewData["FilterFields"] = filterFields;
}

<partial name="_FilterCard" />
```

---

## Complete Example: Items Index Page

### Items/Index.cshtml.cs
```csharp
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Items;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IItemService _itemService;

    public IndexModel(IItemService itemService)
    {
        _itemService = itemService;
    }

    public List<ItemDto> Items { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _itemService.GetAllAsync();
        if (result.Success)
        {
            Items = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(Code))
            {
                Items = Items.Where(i => i.Code.Contains(Code, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Items = Items.Where(i => i.Name.Contains(Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                Items = Items.Where(i => i.Category.Contains(Category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (IsActive.HasValue)
            {
                Items = Items.Where(i => i.IsActive == IsActive.Value).ToList();
            }
        }
    }
}
```

### Items/Index.cshtml
```razor
@page
@model EbikeRental.Web.Pages.Masters.Items.IndexModel
@{
    ViewData["Title"] = "Items";
}

<div class="d-flex justify-content-between align-items-center mb-4">
    <h2><i class="bi bi-box-seam me-2"></i>Items</h2>
    <a asp-page="Info" class="btn btn-primary">
        <i class="bi bi-plus-lg"></i> New Item
    </a>
</div>

<!-- Filter Section -->
<div class="card shadow-sm mb-3">
    <div class="card-header bg-light">
        <h5 class="mb-0"><i class="bi bi-funnel me-2"></i>Filters</h5>
    </div>
    <div class="card-body">
        <form method="get">
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">Code</label>
                    <input type="text" class="form-control" name="code" value="@Model.Code" placeholder="Search by code..." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Name</label>
                    <input type="text" class="form-control" name="name" value="@Model.Name" placeholder="Search by name..." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Category</label>
                    <input type="text" class="form-control" name="category" value="@Model.Category" placeholder="Search by category..." />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Status</label>
                    <select class="form-select" name="isActive">
                        <option value="">All</option>
                        <option value="true" selected="@(Model.IsActive == true)">Active</option>
                        <option value="false" selected="@(Model.IsActive == false)">Inactive</option>
                    </select>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <button type="submit" class="btn btn-primary">
                        <i class="bi bi-search"></i> Search
                    </button>
                    <a asp-page="Index" class="btn btn-secondary">
                        <i class="bi bi-x-circle"></i> Clear
                    </a>
                </div>
            </div>
        </form>
    </div>
</div>

<!-- Table -->
<div class="card shadow-sm">
    <div class="card-body">
        <div class="table-responsive">
            <table class="table table-hover align-middle">
                <thead class="table-light">
                    <tr>
                        <th>Code</th>
                        <th>Name</th>
                        <th>Category</th>
                        <th>Unit</th>
                        <th>Standard Cost</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @if (Model.Items.Any())
                    {
                        @foreach (var item in Model.Items)
                        {
                            <tr>
                                <td><strong>@item.Code</strong></td>
                                <td>@item.Name</td>
                                <td><span class="badge bg-info">@item.Category</span></td>
                                <td>@item.UnitOfMeasure</td>
                                <td>?@item.StandardCost.ToString("N2")</td>
                                <td>
                                    @if (item.IsActive)
                                    {
                                        <span class="badge bg-success">Active</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-secondary">Inactive</span>
                                    }
                                </td>
                                <td>
                                    <a asp-page="Info" asp-route-id="@item.Id" class="btn btn-sm btn-outline-primary">
                                        <i class="bi bi-pencil"></i> Edit
                                    </a>
                                </td>
                            </tr>
                        }
                    }
                    else
                    {
                        <tr>
                            <td colspan="7" class="text-center text-muted py-4">
                                <i class="bi bi-inbox fs-1"></i>
                                <p class="mt-2">No items found</p>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
</div>
```

---

## Filter Patterns by Entity Type

### Simple Master Data (Items, Warehouses, etc.)
**Filter Fields:**
- Code (text)
- Name (text)
- Category (text) - if applicable
- Status (Active/Inactive)

### Transactional Data (PO, PR, Sales Orders)
**Filter Fields:**
- Document Number (text)
- Date Range (from date, to date)
- Status (Draft/Pending/Approved/Rejected)
- Vendor/Customer (dropdown or autocomplete)
- Amount Range (from amount, to amount)

### Assets & Equipment
**Filter Fields:**
- Asset Code (text)
- Serial Number (text)
- Status (Available/Rented/Under Repair/Retired)
- Item/Category (dropdown)
- Warehouse Location (dropdown)

---

## Best Practices

### 1. Always Use Case-Insensitive Search
```csharp
Items = Items.Where(i => i.Code.Contains(Code, StringComparison.OrdinalIgnoreCase)).ToList();
```

### 2. Null/Empty Check Before Filtering
```csharp
if (!string.IsNullOrWhiteSpace(Code))
{
    // Apply filter
}
```

### 3. Clear Button Should Reset All Filters
```razor
<a asp-page="Index" class="btn btn-secondary">
    <i class="bi bi-x-circle"></i> Clear
</a>
```

### 4. Show Result Count
```razor
<div class="text-muted mb-2">
    Showing @Model.Items.Count item(s)
</div>
```

### 5. Collapsible Filter (Optional)
```razor
<div class="card-header bg-light">
    <div class="d-flex justify-content-between align-items-center">
        <h5 class="mb-0"><i class="bi bi-funnel me-2"></i>Filters</h5>
        <button class="btn btn-sm btn-outline-secondary" type="button" data-bs-toggle="collapse" data-bs-target="#filterCollapse">
            <i class="bi bi-chevron-down"></i>
        </button>
    </div>
</div>
<div class="collapse show" id="filterCollapse">
    <!-- Filter content -->
</div>
```

---

## Pages That Need Filters

? **Completed:**
- Masters/Items/Index
- Masters/Assets/Index
- Masters/Warehouses/Index

?? **Todo:**
- Masters/BOM/Index
- Masters/Users/Index
- Purchasing/PR/Index
- Purchasing/PO/Index
- Purchasing/GR/Index
- Production/ProductionOrders/Index
- Rental/Contracts/Index
- Maintenance/RepairOrders/Index

---

## Common Filter Controls

### Text Input
```razor
<input type="text" class="form-control" name="code" value="@Model.Code" placeholder="Search..." />
```

### Select Dropdown
```razor
<select class="form-select" name="status">
    <option value="">All</option>
    <option value="Active" selected="@(Model.Status == "Active")">Active</option>
    <option value="Inactive" selected="@(Model.Status == "Inactive")">Inactive</option>
</select>
```

### Date Input
```razor
<input type="date" class="form-control" name="fromDate" value="@Model.FromDate?.ToString("yyyy-MM-dd")" />
```

### Number Range
```razor
<div class="row">
    <div class="col-6">
        <input type="number" class="form-control" name="minAmount" value="@Model.MinAmount" placeholder="Min" />
    </div>
    <div class="col-6">
        <input type="number" class="form-control" name="maxAmount" value="@Model.MaxAmount" placeholder="Max" />
    </div>
</div>
```

---

## Performance Tips

### 1. Filter at Database Level (Better)
Move filtering to service layer with proper LINQ queries to database.

### 2. Use Pagination
For large datasets, implement pagination along with filtering.

### 3. Consider Using Full-Text Search
For complex search requirements, use database full-text search features.

### 4. Cache Filter Results
Consider caching filter results for frequently accessed data.
