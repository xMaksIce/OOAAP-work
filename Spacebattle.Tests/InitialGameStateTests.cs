using Hwdtech.Ioc;
using Hwdtech;
using Moq;

namespace Spacebattle.Tests;
public class InitialGameStateTests
{
    public InitialGameStateTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }
    [Fact]
    public void SuccessfulGameStateInitializationTest()
    {
        var gameObjects = new Dictionary<(int, int), IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Dictionary.PlayersAndShips",
            (object[] args) => gameObjects
        ).Execute();


        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GameObject.Create",
            (object[] args) => new Mock<IUObject>().Object
        ).Execute();

        int playersAmount = 2;
        int shipsPerPlayer = 3;

        Assert.Empty(IoC.Resolve<Dictionary<(int, int), IUObject>>("Game.Dictionary.PlayersAndShips"));

        new CreateGameObjectsCommand(playersAmount, shipsPerPlayer).Execute();

        Assert.Equal(6, IoC.Resolve<Dictionary<(int, int), IUObject>>("Game.Dictionary.PlayersAndShips").Count);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GameObjectsPerPlayer",
            (object[] args) => (object)shipsPerPlayer).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Players.Amount",
            (object[] args) => (object)playersAmount).Execute();

        var playersIds = new Iterator(new List<object>() { 0, 1 });

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Players.Ids",
            (object[] args) => playersIds).Execute();

        var setPositionCmd = new Mock<Lib.ICommand>();
        setPositionCmd.Setup(c => c.Execute()).Verifiable();
        var setFuelCmd = new Mock<Lib.ICommand>();
        setFuelCmd.Setup(c => c.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SetProperty",
            (object[] args) =>
                new UActionCommand(() =>
                {
                    if ((string)args[1] == "Position") setPositionCmd.Object.Execute();
                    else if ((string)args[1] == "Fuel") setFuelCmd.Object.Execute();

                })
        ).Execute();


        new PlacePlayersShipsCommand().Execute();

        setPositionCmd.Verify(c => c.Execute(), Times.Exactly(6));

        double fuelCapacity = 50.5;
        new SetFuelCapacityCommand(fuelCapacity).Execute();
        setFuelCmd.Verify(c => c.Execute(), Times.Exactly(6));

    }

    [Fact]
    public void IteratorTest()
    {
        var iterator = new Iterator(new List<object>() { 1, 2 });
        Assert.NotEqual(2, (int)iterator.Current);
        iterator.MoveNext();
        Assert.Equal(2, (int)iterator.Current);

        Assert.Throws<System.NotImplementedException>(() => iterator.Dispose());

    }
}
