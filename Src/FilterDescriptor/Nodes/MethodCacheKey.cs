namespace DynamicQuery.Descriptor;

public sealed record MethodCacheKey(int AssemblyId, Type Type, string MethodName, IEnumerable<Type> ArgumentTypes)
    : IEquatable<MethodCacheKey>
{
    private int _hashCode = 0;

    public override int GetHashCode()
    {
        var argTypes = ArgumentTypes?.Select(t => t.TypeHandle.Value);
        return (_hashCode != 0) ? _hashCode : //
         _hashCode = HashCode.Combine(
            AssemblyId,
            Type?.TypeHandle.Value,
            MethodName?.GetHashCode(),
            !(argTypes?.Any() ?? false) ? 0 : argTypes!.Aggregate((a, b) => HashCode.Combine(a, b)));
    }

    public bool Equals(MethodCacheKey? other)
    {
        return GetHashCode() == other?.GetHashCode();
    }
}
 


