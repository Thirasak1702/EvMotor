namespace EbikeRental.Domain.ValueObjects;

public class SerialNumber
{
    public string Value { get; private set; }

    public SerialNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Serial number cannot be empty", nameof(value));

        Value = value.ToUpperInvariant();
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is SerialNumber other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
