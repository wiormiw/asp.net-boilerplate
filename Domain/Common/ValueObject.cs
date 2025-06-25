namespace Domain.Common;

public abstract class ValueObject
{
    protected static bool EqualsOperator(ValueObject left, ValueObject right)
    {
        return left is null ^ right is null ? false : left?.Equals(right!) != false;
    }

    protected static bool NotEqualsOperator(ValueObject left, ValueObject right)
    {
        return !(EqualsOperator(left, right));
    }
    
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;
        
        var other = (ValueObject) obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }
        
        return hash.ToHashCode();
    }
}