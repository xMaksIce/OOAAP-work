using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace Spacebattle.Tests;
public class EndCommandTests
{
    public EndCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Inject", (object[] args) =>
        {
            var gameObj = (IInjectable)args[0];
            var injectedCommand = (Lib.ICommand)args[1];
            gameObj.Inject(injectedCommand);
            return gameObj;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.DeleteProperty", (object[] args) =>
        {
            var gameObj = (UObject)args[0];
            var properties = (List<string>)args[1];
            properties.ForEach(prop => gameObj.DeleteProperty(prop));
            return "";
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.End", (object[] args) =>
        {
            return new EndMoveCommand((IMoveEndable)args[0]);
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.DummyCommand", (object[] args) => new DummyCommand()).Execute();
    }

    [Fact]
    public void BridgeCommandTest()
    {
        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        var bridgeCommand = new BridgeCommand(cmd.Object);
        bridgeCommand.Inject(IoC.Resolve<Lib.ICommand>("Game.Command.DummyCommand"));
        bridgeCommand.Execute();
        cmd.Verify(m => m.Execute(), Times.Never());
    }

    [Fact]
    public void EndMoveCommandTest()
    {
        var cmd = new Mock<Lib.ICommand>();
        var endableCmd = new Mock<IMoveEndable>();
        var bridgeCmd = new BridgeCommand(cmd.Object);
        var gameObj = new Mock<UObject>();
        var propNames = new List<string> { "Velocity" };
        var props = new Dictionary<string, object>();
        gameObj.Setup(o => o.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(props.Add);
        gameObj.Setup(o => o.GetProperty(It.IsAny<string>())).Returns<string>(key => props[key]);
        gameObj.Setup(o => o.DeleteProperty(It.IsAny<string>())).Callback<string>(key => props.Remove(key));
        gameObj.Object.SetProperty("Velocity", new int[] { 12, 7 });
        endableCmd.SetupGet(c => c.Command).Returns(bridgeCmd);
        endableCmd.SetupGet(c => c.Object).Returns(gameObj.Object);
        endableCmd.SetupGet(c => c.Property).Returns(propNames);

        Assert.NotNull(gameObj.Object.GetProperty("Velocity"));
        IoC.Resolve<EndMoveCommand>("Game.Command.End", endableCmd.Object).Execute();
        Assert.Throws<KeyNotFoundException>(() => gameObj.Object.GetProperty("Velocity"));
    }
}
