using Moq;

namespace Spacebattle.Tests;

public class MoveCommandTest
{
    [Fact]
    public void MoveCommandPositive()
    {
        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-7, 3)).Verifiable();
        ICommand moveCommand = new MoveCommand(movable.Object);
        moveCommand.Execute();
        movable.VerifySet(m => m.Position = new Vector(5, 8), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void MoveInvalidPosition()
    {
        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Throws(new InvalidOperationException()).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-7, 3)).Verifiable();
        ICommand moveCommand = new MoveCommand(movable.Object);
        Assert.Throws<InvalidOperationException>(() => moveCommand.Execute());
    }

    [Fact]
    public void MoveInvalidVelocity()
    {
        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(new InvalidOperationException()).Verifiable();
        ICommand moveCommand = new MoveCommand(movable.Object);
        Assert.Throws<InvalidOperationException>(() => moveCommand.Execute());
    }

    [Fact]
    public void MoveInvalidMovement()
    {
        var movable = new Mock<IMovable>();
        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-7, 3)).Verifiable();
        ICommand moveCommand = new MoveCommand(movable.Object);
        movable.SetupSet(m => m.Position = It.IsAny<Vector>()).Throws(new InvalidOperationException());
        Assert.Throws<InvalidOperationException>(() => moveCommand.Execute());
    }
}
