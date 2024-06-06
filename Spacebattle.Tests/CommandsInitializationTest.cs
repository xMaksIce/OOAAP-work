using System.Collections;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace Spacebattle.Tests;
public class CommandsInitializationTests
{
    public CommandsInitializationTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void StartMoveCreates()
    {
        var dummyObject = "";
        var startable = new Mock<IMoveCommandStartable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Startable", (object[] args) => startable.Object).Execute();
        var startMoveStrategy = new CreateStartMove();
        var startMoveCommand = startMoveStrategy.Apply(dummyObject);
        Assert.Equal(typeof(StartMoveCommand), startMoveCommand.GetType());
    }
    [Fact]
    public void EndMoveCreates()
    {
        var dummyObject = "";
        var endable = new Mock<IMoveEndable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Endable", (object[] args) => endable.Object).Execute();
        var endMoveStrategy = new CreateEndMove();
        var endMoveCommand = endMoveStrategy.Apply(dummyObject);
        Assert.Equal(typeof(EndMoveCommand), endMoveCommand.GetType());
    }
    [Fact]
    public void RotateCreates()
    {
        var dummyObject = "";
        var rotatable = new Mock<IRotatable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Rotatable", (object[] args) => rotatable.Object).Execute();
        var rotateStrategy = new CreateRotate();
        var rotateCommand = rotateStrategy.Apply(dummyObject);
        Assert.Equal(typeof(RotateCommand), rotateCommand.GetType());
    }
    [Fact]
    public void ShootCreatesAndExecutes()
    {
        var dummyObject = "";
        var shootable = new Mock<IShootable>();
        var shootImplementation = new Mock<Lib.ICommand>();
        shootImplementation.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Shootable", (object[] args) => shootable.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Shoot.Torpeda", (object[] args) => "torpeda").Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Shoot", (object[] args) => shootImplementation.Object).Execute();
        var shootStrategy = new CreateShoot();
        var shootCommand = (ShootCommand)shootStrategy.Apply(dummyObject);
        shootCommand.Execute();
        Assert.Equal(typeof(ShootCommand), shootCommand.GetType());
        shootImplementation.Verify(cmd => cmd.Execute(), Times.Once());
    }
    [Fact]
    public void GameplayStrategiesRegisteredAndCommandInterprets()
    {
        Hashtable strategies = new()
        {
            ["StartMove"] = new CreateStartMove(),
            ["EndMove"] = new CreateEndMove(),
            ["Rotate"] = new CreateRotate(),
            ["Shoot"] = new CreateShoot()
        };
        var initCommand = new InitializeGameplayStrategies(strategies);
        initCommand.Execute();
        var message = new Mock<IMessageObject>();
        message.SetupGet(msg => msg.Type).Returns("StartMove");
        var dummyObject = "";
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object", (object[] args) => dummyObject).Execute();
        var startable = new Mock<IMoveCommandStartable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Startable", (object[] args) => startable.Object).Execute();
        var queue = new Queue<Lib.ICommand>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.PutCommandInQueue",
        (object[] args) => new ActionCommand((arguments) => queue.Enqueue((Lib.ICommand)arguments[0]), args)).Execute();
        var interpretCommand = new InterpretCommand(message.Object);
        interpretCommand.Execute();
        var startMoveCommand = queue.Dequeue();
        Assert.Equal(typeof(StartMoveCommand), startMoveCommand.GetType());
    }
}
