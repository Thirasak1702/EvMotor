# Partial Views Usage Guide

## Overview
This document describes how to use the shared partial views for consistent UI across the EbikeRental application.

---

## 1. List Pages: `_ListCardLayout.cshtml`

### Purpose
Creates a consistent header with title, icon, and "Create New" button for Index/List pages.

### Usage

```razor
@page
@model YourNamespace.Pages.Module.IndexModel
@{
    ViewData["Title"] = "Your Page Title";
    ViewData["CardTitle"] = "Your Page Title";
    ViewData["CardIcon"] = "bi-icon-name";
    ViewData["CreatePageLink"] = "/Module/Info";
    ViewData["CreateButtonText"] = "New Item";
}

<partial name="_ListCardLayout" />

<!-- Your table content here -->
<table class="table table-hover">
    <!-- ... -->
</table>
</div></div></div>  <!-- Close the card opened by _ListCardLayout -->
```

### Required ViewData Properties
- `CardTitle` (string): The page title displayed
- `CardIcon` (string): Bootstrap icon class (e.g., "bi-list", "bi-file-text")

### Optional ViewData Properties
- `CreatePageLink` (string): URL for "Create New" button. If not provided, button won't show
- `CreateButtonText` (string): Text for "Create New" button. Default: "New Item"

### Example
```razor
@{
    ViewData["CardTitle"] = "Purchase Requisitions";
    ViewData["CardIcon"] = "bi-file-text";
    ViewData["CreatePageLink"] = "/Purchasing/PR/Info";
    ViewData["CreateButtonText"] = "New PR";
}

<partial name="_ListCardLayout" />
```

---

## 2. Form Pages: `_FormCardLayout.cshtml` + `_FormCardLayoutEnd.cshtml`

### Purpose
Creates a consistent form container with header, back button, and card styling for Info/Create/Edit pages.

### Usage

```razor
@page "{id:int?}"
@model YourNamespace.Pages.Module.InfoModel
@{
    ViewData["Title"] = Model.Id.HasValue ? "Edit Item" : "New Item";
    ViewData["FormTitle"] = ViewData["Title"];
    ViewData["FormIcon"] = "bi-pencil";
}

<partial name="_FormCardLayout" />

<form method="post">
    <!-- Your form fields here -->
    
    <div class="d-flex justify-content-end gap-2">
        <a asp-page="./Index" class="btn btn-secondary">
            <i class="bi bi-x-circle me-1"></i> Cancel
        </a>
        <button type="submit" class="btn btn-primary">
            <i class="bi bi-save me-1"></i> Save
        </button>
    </div>
</form>

<partial name="_FormCardLayoutEnd" />

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### Required ViewData Properties
- `FormTitle` (string): The form title. Falls back to `Title` if not provided

### Optional ViewData Properties
- `FormIcon` (string): Bootstrap icon class. Default: "bi-pencil"
- `CardClass` (string): Column width class. Default: "col-md-10"
- `BackLink` (string): Back button URL. Default: "./Index"
- `ShowBackButton` (bool): Show/hide back button. Default: true

### Examples

#### Basic Form
```razor
@{
    ViewData["FormTitle"] = "New Item";
    ViewData["FormIcon"] = "bi-box-seam";
}

<partial name="_FormCardLayout" />
<form method="post">
    <!-- form fields -->
</form>
<partial name="_FormCardLayoutEnd" />
```

#### Form with Custom Width
```razor
@{
    ViewData["FormTitle"] = "Edit Asset";
    ViewData["FormIcon"] = "bi-box-seam";
    ViewData["CardClass"] = "col-md-8";  // Narrower form
}

<partial name="_FormCardLayout" />
<form method="post">
    <!-- form fields -->
</form>
<partial name="_FormCardLayoutEnd" />
```

#### Form without Back Button
```razor
@{
    ViewData["FormTitle"] = "Setup Wizard";
    ViewData["FormIcon"] = "bi-gear";
    ViewData["ShowBackButton"] = false;
}

<partial name="_FormCardLayout" />
<form method="post">
    <!-- form fields -->
</form>
<partial name="_FormCardLayoutEnd" />
```

---

## Best Practices

### 1. Always Close Tags Properly
```razor
<!-- ? CORRECT -->
<partial name="_FormCardLayout" />
<form method="post">
    <!-- content -->
</form>
<partial name="_FormCardLayoutEnd" />

<!-- ? WRONG - Missing closing partial -->
<partial name="_FormCardLayout" />
<form method="post">
    <!-- content -->
</form>
```

### 2. Consistent Icons
Use Bootstrap Icons consistently:
- Items/Products: `bi-box-seam`
- Documents: `bi-file-text`
- Users: `bi-people`
- Warehouses: `bi-building`
- Settings: `bi-gear`
- BOM: `bi-diagram-3`

### 3. Button Styling
Use consistent button patterns:
```razor
<!-- Cancel button -->
<a asp-page="./Index" class="btn btn-secondary">
    <i class="bi bi-x-circle me-1"></i> Cancel
</a>

<!-- Save button -->
<button type="submit" class="btn btn-primary">
    <i class="bi bi-save me-1"></i> Save
</button>
```

### 4. Form Layout
- Use `row` and `col-md-*` for responsive layouts
- Add `mb-3` for spacing between form groups
- Use `form-label`, `form-control`, `form-select` classes

---

## Common Patterns

### Index Page Pattern
```razor
@page
@model IndexModel
@{
    ViewData["CardTitle"] = "Items";
    ViewData["CardIcon"] = "bi-box-seam";
    ViewData["CreatePageLink"] = "./Info";
    ViewData["CreateButtonText"] = "New Item";
}

<partial name="_ListCardLayout" />

<table class="table table-hover">
    <thead>
        <tr>
            <th>Code</th>
            <th>Name</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model.Items)
        {
            <tr>
                <td>@item.Code</td>
                <td>@item.Name</td>
                <td>
                    <a asp-page="./Info" asp-route-id="@item.Id" class="btn btn-sm btn-outline-primary">
                        <i class="bi bi-pencil"></i> Edit
                    </a>
                </td>
            </tr>
        }
    </tbody>
</table>
            </div>
        </div>
    </div>
```

### Info Page Pattern
```razor
@page "{id:int?}"
@model InfoModel
@{
    ViewData["FormTitle"] = Model.Id.HasValue ? "Edit Item" : "New Item";
    ViewData["FormIcon"] = "bi-box-seam";
}

<partial name="_FormCardLayout" />

<form method="post">
    <input type="hidden" asp-for="Item.Id" />
    
    <div class="row mb-3">
        <div class="col-md-6">
            <label asp-for="Item.Code" class="form-label">Code</label>
            <input asp-for="Item.Code" class="form-control" />
            <span asp-validation-for="Item.Code" class="text-danger"></span>
        </div>
        <div class="col-md-6">
            <label asp-for="Item.Name" class="form-label">Name</label>
            <input asp-for="Item.Name" class="form-control" />
            <span asp-validation-for="Item.Name" class="text-danger"></span>
        </div>
    </div>
    
    <div class="d-flex justify-content-end gap-2">
        <a asp-page="./Index" class="btn btn-secondary">
            <i class="bi bi-x-circle me-1"></i> Cancel
        </a>
        <button type="submit" class="btn btn-primary">
            <i class="bi bi-save me-1"></i> Save
        </button>
    </div>
</form>

<partial name="_FormCardLayoutEnd" />

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

---

## Migration Guide

### Converting Old Form Pages to Use Partials

**Before:**
```razor
@page
@model InfoModel
@{
    ViewData["Title"] = "Edit Item";
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h2>@ViewData["Title"]</h2>
    <a asp-page="Index" class="btn btn-outline-secondary">Back to List</a>
</div>

<div class="card shadow-sm">
    <div class="card-header bg-primary text-white">
        <h4 class="mb-0"><i class="bi bi-box-seam me-2"></i>@ViewData["Title"]</h4>
    </div>
    <div class="card-body">
        <form method="post">
            <!-- form content -->
        </form>
    </div>
</div>
```

**After:**
```razor
@page
@model InfoModel
@{
    ViewData["Title"] = "Edit Item";
    ViewData["FormTitle"] = ViewData["Title"];
    ViewData["FormIcon"] = "bi-box-seam";
}

<partial name="_FormCardLayout" />

<form method="post">
    <!-- form content -->
</form>

<partial name="_FormCardLayoutEnd" />
```

**Changes:**
1. ? Add `FormTitle` and `FormIcon` to ViewData
2. ? Replace header and card opening with `<partial name="_FormCardLayout" />`
3. ? Remove closing `</div>` tags and add `<partial name="_FormCardLayoutEnd" />`
4. ? Keep form and content unchanged

---

## Troubleshooting

### Issue: Form buttons not aligned properly
**Solution:** Ensure you're using proper Bootstrap classes:
```razor
<div class="d-flex justify-content-end gap-2">
    <!-- buttons -->
</div>
```

### Issue: Card not displaying correctly
**Solution:** Check that you have both start and end partials:
- `<partial name="_FormCardLayout" />` at the beginning
- `<partial name="_FormCardLayoutEnd" />` at the end

### Issue: Back button links to wrong page
**Solution:** Override the BackLink property:
```razor
@{
    ViewData["BackLink"] = "/Custom/Path";
}
```

---

## File Locations
- `_FormCardLayout.cshtml`: `/Pages/Shared/_FormCardLayout.cshtml`
- `_FormCardLayoutEnd.cshtml`: `/Pages/Shared/_FormCardLayoutEnd.cshtml`
- `_ListCardLayout.cshtml`: `/Pages/Shared/_ListCardLayout.cshtml`
