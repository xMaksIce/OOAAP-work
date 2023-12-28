using System.Collections;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Tests;
public class RegisterHandler
{
    [Fact]
    public void RegisterHandlerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        
        Dictionary<Type, Dictionary<Type, Lib.ICommand>> tree = new();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Handle.GetTree", (object[] args) => tree).Execute();

        Mock<IMovable> movable = new();
        MoveCommand moveCommand = new(movable.Object);
        InvalidOperationException firstException = new();
        NotFiniteNumberException secondException = new();
        Mock<Lib.ICommand> firstHandler = new();
        Mock<Lib.ICommand> secondHandler = new();

        new RegisterExceptionHandler(moveCommand, firstException, firstHandler.Object).Execute();
        new RegisterExceptionHandler(moveCommand, secondException, secondHandler.Object).Execute();

        var actualTree = IoC.Resolve<Dictionary<Type, Dictionary<Type, Lib.ICommand>>>("Game.Handle.GetTree");

        Type moveCType = typeof(MoveCommand);
        Type firstExcType = typeof(InvalidOperationException);
        Type secondExcType = typeof(NotFiniteNumberException);
        Assert.Equal(actualTree[moveCType][firstExcType], firstHandler.Object);
        Assert.Equal(actualTree[moveCType][secondExcType], secondHandler.Object);
    }
}
