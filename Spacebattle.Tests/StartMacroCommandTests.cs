using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Lib.Tests;

public class StartMacroCommandTests
{
    [Fact]
    public void RegistersMacroCommandAndQueue_WhenAddingItIntoQueue_ThenCompareTakenAndGivenCommands()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var moveCommandStartable = new Mock<IMoveCommandStartable>();

        moveCommandStartable.SetupGet(mcs => mcs.Target).Returns( new Mock<IUObject>().Object );
        moveCommandStartable.SetupGet(mcs => mcs.Property).Returns( new Dictionary<string, object>() );

        string dependencyName = "Rotate";

        var startMacroCommand = new StartMacroCommand(dependencyName, moveCommandStartable.Object);

        var qMock = new Mock<IQueue>();
        var qReal = new Queue<ICommand>();

        qMock.Setup(q => q.Take()).Returns(() => qReal.Dequeue());
        qMock.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(
            (ICommand cmd) =>
            {
                qReal.Enqueue(cmd);
            }
        );

        var macroCommand = new Mock<ICommand>().Object;

        string depPath = string.Format("Game.Commands.{0}", dependencyName);

        IoC.Resolve<Hwdtech.ICommand>(
        "IoC.Register",
        depPath,
        (object[] args) =>
        {
            return macroCommand;
        }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Commands.Injectable",
            (object[] args) =>
            {
                return macroCommand;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return qMock.Object;
            }
        ).Execute();
    

        startMacroCommand.Execute();

        Assert.Equal(macroCommand, qMock.Object.Take());
    }
}
