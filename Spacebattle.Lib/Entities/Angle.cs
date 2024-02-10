public class Angle
{
    private static readonly int MaxPosition = 360;
    public Angle(int InclinarAngle) => _inclinarAngle = InclinarAngle;
    private readonly int _inclinarAngle;

    public static Angle operator +(Angle firstAngle, Angle secondAngle)
    {
        var resultAngle = new Angle((firstAngle._inclinarAngle + secondAngle._inclinarAngle) % MaxPosition);
        return resultAngle;
    }
    public override bool Equals(object? obj)
    {
        return obj is Angle angle && angle._inclinarAngle == _inclinarAngle;
    }

    public override int GetHashCode()
    {
        return _inclinarAngle.GetHashCode();
    }

}
