# Tailwind CSS Setup Guide

## Overview
This project now uses **Tailwind CSS v3** (free version) via CDN for styling, replacing Bootstrap.

## What's Implemented

### 1. Tailwind CSS Integration
- **CDN-based setup** (no build process required)
- Custom configuration for Poppins font family
- Bootstrap Icons for icon support

### 2. Converted Pages
- ? Layout (_Layout.cshtml)
- ? Sidebar with collapsible menu (_Sidebar.cshtml)
- ? Login page (Auth/Login.cshtml)
- ? Dashboard (Dashboard/Index.cshtml)
- ? Items Master (Masters/Items/Index.cshtml, Info.cshtml)
- ? Form Card Layout components

### 3. Custom JavaScript
- Menu toggle functionality for sidebar
- User dropdown menu
- No jQuery or Bootstrap JS dependencies

## File Structure

```
EbikeRental.Web/
??? Pages/
?   ??? Shared/
?   ?   ??? _Layout.cshtml          (Main layout with Tailwind)
?   ?   ??? _Sidebar.cshtml         (Sidebar with custom JS)
?   ?   ??? _FormCardLayout.cshtml  (Form container start)
?   ?   ??? _FormCardLayoutEnd.cshtml
?   ??? Auth/
?   ?   ??? Login.cshtml            (Login page)
?   ??? Dashboard/
?   ?   ??? Index.cshtml            (Dashboard)
?   ??? Masters/Items/
?       ??? Index.cshtml            (Items list)
?       ??? Info.cshtml             (Item form)
??? wwwroot/css/
?   ??? custom.css                  (Minimal custom styles)
?   ??? site.css                    (Empty/minimal)
??? package.json                     (npm config for future use)
??? tailwind.config.js              (Tailwind configuration)
```

## Key Tailwind Classes Used

### Layout
- `flex`, `flex-col`, `min-h-screen` - Flexbox layout
- `bg-gray-50`, `bg-white` - Background colors
- `p-6`, `px-4`, `py-2` - Padding utilities
- `rounded-lg`, `rounded-2xl` - Border radius
- `shadow-sm`, `shadow-lg` - Box shadows

### Components
- **Buttons**: `bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition`
- **Inputs**: `w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500`
- **Cards**: `bg-white rounded-lg shadow-sm p-6`
- **Tables**: `w-full divide-y divide-gray-200`
- **Badges**: `px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded-full`

### Sidebar
- `bg-gradient-to-br from-blue-600 to-blue-800` - Gradient background
- `hover:bg-white hover:bg-opacity-10` - Hover effects
- Custom JavaScript for menu collapse

## How to Use Tailwind in New Pages

### 1. Basic Page Structure
```razor
@page
@model YourModel
@{
    ViewData["Title"] = "Page Title";
}

<div class="mb-6">
    <h1 class="text-2xl font-bold text-gray-800">Page Title</h1>
</div>

<div class="bg-white rounded-lg shadow-sm p-6">
    <!-- Your content here -->
</div>
```

### 2. Form with Tailwind
```razor
<form method="post">
    <div class="mb-4">
        <label class="block text-sm font-medium text-gray-700 mb-2">Label</label>
        <input type="text" 
               class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
               placeholder="Enter value" />
    </div>
    
    <div class="flex justify-end gap-2">
        <button type="button" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg">Cancel</button>
        <button type="submit" class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">Save</button>
    </div>
</form>
```

### 3. Data Table
```razor
<div class="overflow-x-auto">
    <table class="w-full">
        <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-700 uppercase">Column</th>
            </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
            <tr class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">Data</td>
            </tr>
        </tbody>
    </table>
</div>
```

## Common Patterns

### Status Badges
```razor
@if (isActive)
{
    <span class="px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded-full">Active</span>
}
else
{
    <span class="px-2 py-1 text-xs font-medium bg-gray-100 text-gray-800 rounded-full">Inactive</span>
}
```

### Cards
```razor
<div class="bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-lg shadow-lg p-6">
    <div class="text-sm font-medium opacity-90 mb-2">Label</div>
    <div class="text-3xl font-bold mb-1">@Model.Count</div>
    <p class="text-sm opacity-80">Description</p>
</div>
```

### Grid Layout
```razor
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
    <!-- Grid items -->
</div>
```

## Responsive Design

Tailwind uses mobile-first breakpoints:
- `sm:` - 640px and up
- `md:` - 768px and up
- `lg:` - 1024px and up
- `xl:` - 1280px and up
- `2xl:` - 1536px and up

Example:
```html
<div class="w-full md:w-1/2 lg:w-1/3">
    <!-- Full width on mobile, half on tablet, third on desktop -->
</div>
```

## Color Palette

### Primary Colors (Blue)
- `bg-blue-600`, `text-blue-600` - Primary
- `bg-blue-700`, `hover:bg-blue-700` - Hover/Active

### Status Colors
- **Success**: `bg-green-500`, `text-green-800`, `bg-green-100`
- **Warning**: `bg-yellow-500`, `text-yellow-800`, `bg-yellow-100`
- **Danger**: `bg-red-500`, `text-red-800`, `bg-red-100`
- **Info**: `bg-cyan-500`, `text-cyan-800`, `bg-cyan-100`

### Neutral Colors
- `bg-gray-50` - Page background
- `bg-gray-100` - Light backgrounds
- `bg-gray-700`, `bg-gray-800` - Dark backgrounds
- `text-gray-600`, `text-gray-700`, `text-gray-800` - Text colors

## Resources

- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [Tailwind CSS Cheat Sheet](https://nerdcave.com/tailwind-cheat-sheet)
- [Bootstrap Icons](https://icons.getbootstrap.com/)

## Migration Notes

### What Was Removed
- ? Bootstrap CSS and JS
- ? Bootstrap bundle (collapse, dropdown JS)
- ? Custom CSS that duplicated Bootstrap

### What Was Added
- ? Tailwind CSS CDN
- ? Custom JavaScript for sidebar menu
- ? Bootstrap Icons (kept for icons only)
- ? Modern gradient designs
- ? Better responsive utilities

## Future Improvements

If you want to optimize for production:

1. **Install Tailwind CLI** (optional, for build process)
   ```bash
   npm install -D tailwindcss
   npx tailwindcss -i ./Styles/input.css -o ./wwwroot/css/output.css --minify
   ```

2. **Use JIT mode** for smaller file sizes

3. **Purge unused styles** in production build

## Troubleshooting

### Icons not showing?
Make sure Bootstrap Icons CDN is included in _Layout.cshtml:
```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
```

### Menu not collapsing?
Check that the JavaScript in _Sidebar.cshtml is loaded correctly.

### Styles not applying?
Clear browser cache or do a hard refresh (Ctrl+Shift+R).
