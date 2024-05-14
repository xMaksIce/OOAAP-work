using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Tests;
public class GameCommandTests
{
    public GameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }
    [Fact]
    public void GameCommandExecutesAllCommands_HandlerStopsException()
    {
        var scope = IoC.Resolve<object>("Scopes.Current");
        int timeQ = 10_000;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameCommand.TimeQuantum",
            (object[] args) => (object)timeQ
        ).Execute();
        var goodHandleCmd = new Mock<Lib.ICommand>();
        goodHandleCmd.Setup(c => c.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handle",
            (object[] args) => goodHandleCmd.Object
        ).Execute();

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(c => c.Execute()).Callback(() => Thread.Sleep(50)).Verifiable();

        var cmdE = new Mock<Lib.ICommand>();
        cmdE.Setup(c => c.Execute()).Throws(new Exception()).Verifiable();

        var q = new Queue<Lib.ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmdE.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);

        var gc = new GameCommand(scope, q);
        gc.Execute();

        cmd.Verify(c => c.Execute(), Times.Exactly(4));
        cmdE.Verify(c => c.Execute(), Times.Once());
        Assert.Empty(q);

    }

    [Fact]
    public void GameCommandDoesNotExecutesALLCommands()
    {
        var scope = IoC.Resolve<object>("Scopes.Current");
        int timeQ = 50;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameCommand.TimeQuantum",
            (object[] args) => (object)timeQ
        ).Execute();

        var goodHandleCmd = new Mock<Lib.ICommand>();
        goodHandleCmd.Setup(c => c.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handle",
            (object[] args) => goodHandleCmd.Object
        ).Execute();
        var cmd = new Mock<Lib.ICommand>();

        cmd.Setup(c => c.Execute()).Callback(() => Thread.Sleep(50)).Verifiable();
        var q = new Queue<Lib.ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);

        var gc = new GameCommand(scope, q);
        gc.Execute();

        cmd.Verify(c => c.Execute(), Times.Once());

    }

    [Fact]
    public void GameCommandDoesNotExecuteANYCommand()
    {
        var scope = IoC.Resolve<object>("Scopes.Current");
        int timeQ = 0;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameCommand.TimeQuantum",
            (object[] args) => (object)timeQ
        ).Execute();
        var goodHandleCmd = new Mock<Lib.ICommand>();
        goodHandleCmd.Setup(c => c.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handle",
            (object[] args) => goodHandleCmd.Object
        ).Execute();
        var cmd = new Mock<Lib.ICommand>();

        cmd.Setup(c => c.Execute()).Verifiable();
        var q = new Queue<Lib.ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);


        var gc = new GameCommand(scope, q);
        gc.Execute();

        cmd.Verify(c => c.Execute(), Times.Exactly(0));

    }

    [Fact]
    public void DefaultExceptionHandlerTest()
    {
        var scope = IoC.Resolve<object>("Scopes.Current");
        int timeQ = 10_000;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameCommand.TimeQuantum",
            (object[] args) => (object)timeQ
        ).Execute();

        var cmdE = new Mock<Lib.ICommand>();
        cmdE.Setup(c => c.Execute()).Throws(new Exception()).Verifiable();

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(c => c.Execute()).Callback(() => Thread.Sleep(500)).Verifiable();

        var defaultHandleCmd = new Mock<Lib.ICommand>();
        defaultHandleCmd.Setup(c => c.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handle",
            (object[] args) => defaultHandleCmd.Object
        ).Execute();

        var q = new Queue<Lib.ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmdE.Object);
        q.Enqueue(cmd.Object);

        var gc = new GameCommand(scope, q);
        Assert.Throws<Exception>(() => gc.Execute());

        cmd.Verify(c => c.Execute(), Times.Exactly(3));
        cmdE.Verify(c => c.Execute(), Times.Once());
        defaultHandleCmd.Verify(c => c.Execute(), Times.Once());

        Assert.NotEmpty(q);
    }
}
