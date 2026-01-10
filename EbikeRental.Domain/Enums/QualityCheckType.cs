namespace EbikeRental.Domain.Enums;

public enum QualityCheckType
{
    Incoming = 1,      // Incoming QC for received materials
    InProcess = 2,     // In-process QC during production
    Final = 3          // Final QC (VQC) for finished goods
}
