namespace EbikeRental.Domain.Entities;

public class GoodsReceiptItem
{
    public int Id { get; set; }
    public int GoodsReceiptId { get; set; }
    public GoodsReceipt? GoodsReceipt { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public bool IsAccepted { get; set; } = true;
    
    // ? Phase 1: Barcode & Serial Number Tracking
    public string? Barcode { get; set; }        // ? NEW: Barcode per GR line
    public string? SerialNumber { get; set; }   // ? NEW: Serial Number per GR line
    
    // QC Reference
    public bool RequiresQC { get; set; } = false;
    public string? QCStatus { get; set; } // Pending, Passed, Failed, Conditional
    public int? QualityCheckId { get; set; }
}
