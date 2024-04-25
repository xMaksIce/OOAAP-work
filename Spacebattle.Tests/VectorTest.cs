using System.Collections;

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

    [Fact]
    public void DifferentSizesErrorSubstraction()
    {
        Vector a = new(12, 7, 3);
        Vector b = new(12, 7);
        Assert.Throws<InvalidOperationException>(() => a - b);
    }

    [Fact]
    public void SubsractTwoVectorsGetAnother()
    {
        Vector a = new(12, 7, 3);
        Vector b = new(10, 5, 1);

        Vector expected = new(2, 2, 2);
        Vector actual = a - b;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConcatTwoVectorsGetBiggerOne()
    {
        Vector a = new(1, 2);
        Vector b = new(3, 4, 5);

        Vector expected = new(1, 2, 3, 4, 5);
        Vector actual = Vector.Concat(a, b);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetVectorEnumerator()
    {
        int[] initialArray = new int[] { 1, 2 };
        Vector v = new Vector(initialArray);

        IEnumerator expected = ((IEnumerable<int>)initialArray).GetEnumerator();
        var actual = Vector.GetEnumerator(v);

        Assert.Equal(expected.GetType(), actual.GetType());
    }
}
