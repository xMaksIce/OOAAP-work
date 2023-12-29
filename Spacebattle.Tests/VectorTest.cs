namespace Spacebattle.Tests;

public class VectorTest
{
    [Fact]
    public void EqualsAndHashCodeSymmetic()
    {
        Vector a = new(12, 7);
        Vector b = new(12, 7);
        Assert.Equal(a, b);
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void NotEqualsAndHashCodeSymmetic()
    {
        Vector a = new(12, 7);
        Vector b = new(7, 12);
        Assert.NotEqual(a, b);
        Assert.False(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void DifferentSizesError()
    {
        Vector a = new(12, 7, 3);
        Vector b = new(12, 7);
        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void ZeroArguments()
    {
        int[]? ints = null;
        Assert.Throws<ArgumentNullException>(() => new Vector(ints!));
    }
}
