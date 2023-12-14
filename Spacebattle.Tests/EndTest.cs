using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace Spacebattle.Tests;
public class EndCommandTests
{

    [Fact]
    public void BridgeCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.DummyCommand", (object[] args) => new DummyCommand()).Execute();

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        var bridgeCommand = new BridgeCommand(cmd.Object);
        bridgeCommand.Inject(IoC.Resolve<Lib.ICommand>("Game.Command.DummyCommand"));
        bridgeCommand.Execute();
        cmd.Verify(m => m.Execute(), Times.Never());
    }
    
}
