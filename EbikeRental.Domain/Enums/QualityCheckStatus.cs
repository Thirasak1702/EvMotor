namespace EbikeRental.Domain.Enums;

public enum QualityCheckStatus
{
    Pending = 1,       // Awaiting inspection
    InProgress = 2,    // Inspection in progress
    Passed = 3,        // QC passed
    Failed = 4,        // QC failed
    Conditional = 5    // Passed with conditions/remarks
}
