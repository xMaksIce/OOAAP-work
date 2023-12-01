using Spacebattle.Lib;

namespace Spacebattle.Lib;


public interface IRotatable
{
    public Angle HorizonInclinationAngle { get; set; }
    public Angle AngularVelocity { get; }
}
