using Spacebattle.Lib;

public class RotateCommand : ICommand
{
    private readonly IRotatable rotatable;
    public RotateCommand(IRotatable rotatable)
    {
        this.rotatable = rotatable;
    }

    public void Execute()
    {
        rotatable.HorizonInclinationAngle += rotatable.AngularVelocity;
    }
}
