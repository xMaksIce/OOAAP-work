namespace AngleClassTest;


public class AngleClassTest
{
    //  при прибавлении 30 град при позиции 60 град позиция будет 90 град 
    [Theory]
    [InlineData(30, 60)]
    public void AddAngleGetChangedPosition(int StartAngle, int TurnAngle)
    {
        var start = new Angle(StartAngle);
        var turn = new Angle(TurnAngle);
        var actual = start + turn;
        Angle expected = new Angle(90);
        Assert.True(actual.Equals(expected));
    }

    // hashcode разные
    [Theory]
    [InlineData(30, 60)]
    public void CompareTwoAnglesGetTrue(int firstAngle, int secondAngle)
    {

        var first = new Angle(firstAngle);
        var second = new Angle(secondAngle);
        Assert.False(first.GetHashCode() == second.GetHashCode());
    }

    // сравниваем хешкоды углов с одинаковыми углами
    [Theory]
    [InlineData(30, 30)]
    public void CompareHashCodesOfTwoAnglesGetTrue(int firstAngle, int secondAngle)
    {

        var first = new Angle(firstAngle);
        var second = new Angle(secondAngle);
        Assert.True(first.GetHashCode() == second.GetHashCode());
    }

    // сравнение когда один из углов null
    [Theory]
    [InlineData(30, null)]
    public void NullCheckOfTwoAnglesGetFalse(int firstAngle, int? secondAngle)
    {
        var first = new Angle(firstAngle);
        Assert.False(first.Equals(secondAngle));
    }
}
