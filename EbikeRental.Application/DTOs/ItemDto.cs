using EbikeRental.Domain.Enums;

namespace EbikeRental.Application.DTOs;

public class ItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal StandardCost { get; set; }
    public bool IsActive { get; set; }
    
    // Tracking Flags
    public bool IsBarcode { get; set; }          // Flag for barcode tracking
    public bool IsSerial { get; set; }           // Flag for serial number tracking
    public bool IsBatch { get; set; }            // Flag for batch number tracking
    public bool IsExpiry { get; set; }           // Flag for expiry date tracking
    
    public string? Barcode { get; set; }
    public string TrackingMethod { get; set; } = "None"; // Deprecated - keeping for backward compatibility
    
    public DateTime CreatedAt { get; set; }
}
