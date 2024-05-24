using Hwdtech.Ioc;
using Hwdtech;
using Moq;

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
    public void InitialGameStateTest()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.CreateGameObjects",
        (object[] args) => new CreateGameObjectsStrategy().Run(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.ArrangePlayersSpaceships",
        (object[] args) => new ArrangePlayersSpaceshipsStrategy().Run(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SetFuelCapacity",
        (object[] args) => new SetFuelCapacityCommand().Execute();
        ).Execute();
    }
}
