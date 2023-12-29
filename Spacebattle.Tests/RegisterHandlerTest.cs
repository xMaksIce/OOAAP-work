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
        
        Hashtable tree = new();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Handle.GetTree", (object[] args) => tree).Execute();

        Mock<IMovable> movable = new();
        MoveCommand moveCommand = new(movable.Object);
        NotFiniteNumberException exception = new();
        Mock<Lib.ICommand> firstHandler = new();
        Mock<Lib.ICommand> secondHandler = new();

        new RegisterExceptionHandler(moveCommand, firstHandler.Object).Execute();
        new RegisterExceptionHandler(exception, secondHandler.Object).Execute();

        var actualTree = IoC.Resolve<Hashtable>("Game.Handle.GetTree");

        Type moveCType = typeof(MoveCommand);
        Type excType = typeof(NotFiniteNumberException);
        Assert.Equal(actualTree[moveCType], firstHandler.Object);
        Assert.Equal(actualTree[excType], secondHandler.Object);
    }
}
