namespace Spacebattle.Lib;

public class Vector
{
    private readonly int[] _values;
    public int Size => _values.Length;
    
    public Vector(params int[] values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    public static Vector operator +(Vector left, Vector right)
    {
        if (left.Size != right.Size)
            throw new InvalidOperationException("Размеры векторов не совпадают");
        var resultValues = left._values.Zip(right._values, (a, b) => a + b).ToArray();
        var result = new Vector(resultValues);
        return result;
    }

    public override bool Equals(object? obj)
    {
        var otherVector = obj as Vector;
        return otherVector!._values.SequenceEqual(_values);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = _values.Aggregate((hash, value) => hash * 23 + value.GetHashCode());
        return hash;
    }
}
