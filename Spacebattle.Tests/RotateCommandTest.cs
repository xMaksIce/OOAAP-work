namespace Spacebattle.Lib.Tests;

using Moq;
using Spacebattle.Lib;

public class RotateCommandTest
{
    // попытка сдвинуть корабль, у которого невозможно прочитать значение угла наклона к горизонту, приводит к ошибке.
    [Fact]
    public void CannotGetAngleThrowsException()
    {
        var rotatable = new Mock<IRotatable>();
        rotatable.SetupGet(p => p.HorizonInclinationAngle).Throws(new Exception()).Verifiable();
    }

    // попытка сдвинуть корабль, у которого невозможно изменить угол наклона к горизонту, приводит к ошибке.
    [Theory]
    [InlineData(45)]
    public void CannotSetAngleThrowsException(int angle)
    {
        var rotatable = new Mock<IRotatable>();
        rotatable.SetupSet(x => x.HorizonInclinationAngle = new Angle(angle)).Throws(new Exception());
    }

    // попытка сдвинуть корабль, у которого невозможно прочитать значение угловой скорости, приводит к ошибке.

    [Fact]
    public void CannotGetAngularVelocityThrowsException()
    {
        var rotatable = new Mock<IRotatable>();
        rotatable.SetupGet(r => r.AngularVelocity).Throws(new Exception()).Verifiable();
    }

    // корабль, который находится под углом к горизонту в 45 градусов имеет угловую скорость 90 градусов. 
    // В результате поворота корабль оказывается под углом 135 градусов к горизонту.    
    [Theory]
    [InlineData(45, 90)]
    public void HasSomeHorizontalAngleAndAngleVelocityReturnChangedHorizontalAngle(int startAngle, int angleVelocity)
    {
        var rotatable = new Mock<IRotatable>();

        rotatable.SetupGet(r => r.HorizonInclinationAngle).Returns(new Angle(startAngle)).Verifiable();

        rotatable.SetupGet(r => r.AngularVelocity).Returns(new Angle(angleVelocity)).Verifiable();

        ICommand rotateCommand = new RotateCommand(rotatable.Object);

        rotateCommand.Execute();

        rotatable.VerifySet(r => r.HorizonInclinationAngle = new Angle(startAngle + angleVelocity), Times.Once);

    }
}
