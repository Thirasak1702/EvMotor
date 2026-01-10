namespace EbikeRental.Shared.Helpers;

public static class AssetHelper
{
    public static string GenerateAssetCode(string prefix, int sequence)
    {
        return $"{prefix}-{sequence:D6}";
    }

    public static string FormatSerialNumber(string serialNumber)
    {
        return serialNumber.ToUpperInvariant().Trim();
    }

    public static bool ValidateSerialNumber(string serialNumber)
    {
        return !string.IsNullOrWhiteSpace(serialNumber) && 
               serialNumber.Length >= 5 && 
               serialNumber.Length <= 50;
    }
}
