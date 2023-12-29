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
        InvalidOperationException firstException = new();
        NotFiniteNumberException secondException = new();
        NotFiniteNumberException thirdException = new(); 
        Mock<Lib.ICommand> firstHandler = new();
        Mock<Lib.ICommand> secondHandler = new();
        Mock<Lib.ICommand> thirdHandler = new();

        new RegisterExceptionHandler(new List<object>(){ moveCommand, firstException }, firstHandler.Object).Execute();
        new RegisterExceptionHandler(new List<object>(){ secondException }, secondHandler.Object).Execute();
        new RegisterExceptionHandler(new List<object>(){ moveCommand, thirdException }, thirdHandler.Object).Execute();
        
        Hashtable actualTree = IoC.Resolve<Hashtable>("Game.Handle.GetTree");

        Type moveCType = typeof(MoveCommand);
        Type firstExcType = typeof(InvalidOperationException);
        Type secondExcType = typeof(NotFiniteNumberException);
        Type thirdExcType = typeof(NotFiniteNumberException);

        Hashtable moveCommandBranch = new(){
            {firstExcType, firstHandler.Object},
            {thirdExcType, thirdHandler.Object}};

        Assert.Equal(moveCommandBranch, actualTree[moveCType]);
        Assert.Equal(secondHandler.Object, actualTree[secondExcType]);
    }
}
