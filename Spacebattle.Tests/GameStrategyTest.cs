using System.Collections;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace Spacebattle.Tests;

public class GameStrategyTests
{
    public GameStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void GameCreates()
    {
        int gameTick = 64;
        object scope = "scope";
        Queue<Lib.ICommand> queue = new();
        int tickInGame = 0;
        object scopeInGame = "";
        Queue<Lib.ICommand> queueInGame = new();
        queueInGame.Enqueue(new Mock<Lib.ICommand>().Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameCommandImplementation", (object[] args) =>
        {
            tickInGame = (int)args[0];
            scopeInGame = args[1];
            queueInGame = (Queue<Lib.ICommand>)args[2];
            return new Mock<Lib.ICommand>().Object;
        }).Execute();
        var createGameStrategy = new CreateGame();
        var gameFromStrategy = (GameCommand)createGameStrategy.Apply(gameTick, scope, queue);
        gameFromStrategy.Execute();
        Assert.Equal(gameTick, tickInGame);
        Assert.Equal(scope, scopeInGame);
        Assert.Equal(queue, queueInGame);
    }
    [Fact]
    public void GameDeletes()
    {
        int gameID = 16;
        Hashtable scopes = new() { [gameID] = "scope" };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scopes.List", (object[] args) => scopes).Execute();
        var deleteGameCommand = new DeleteGame(gameID);
        deleteGameCommand.Execute();
        Assert.False(scopes.ContainsKey(gameID));
    }
    [Fact]
    public void GameDidNotExist()
    {
        int gameID = 16;
        Hashtable scopes = new() { [gameID] = "scope" };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scopes.List", (object[] args) => scopes).Execute();
        Hashtable prevScopes = (Hashtable)scopes.Clone();
        int nonExistentGameID = 32;
        var deleteGameCommand = new DeleteGame(nonExistentGameID);
        deleteGameCommand.Execute();
        Assert.Equal(scopes, prevScopes);
    }
    [Fact]
    public void CommandIsEnqueuedAndDequeued()
    {
        int gameID = 16;
        var command = new Mock<Lib.ICommand>();
        command.Setup(cmd => cmd.Execute()).Verifiable();
        var queue = new Queue<Lib.ICommand>();
        Hashtable gamesQueues = new() { [gameID] = queue };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => gamesQueues[(int)args[0]]).Execute();
        var putInQueueCommand = new PutInQueue(gameID, command.Object);
        putInQueueCommand.Execute();
        var takeFromQueueStrategy = new TakeFromQueue();
        var dequeuedCommand = (Lib.ICommand)takeFromQueueStrategy.Apply(gameID);
        dequeuedCommand.Execute();
        command.Verify(cmd => cmd.Execute(), Times.Once());
    }
    [Fact]
    public void ObjectReadsAndDeletes()
    {
        int gameID = 16;
        int objectID = 128;
        object obj = "object";
        Hashtable pool = new() { [objectID] = obj };
        Hashtable gamesPools = new() { [gameID] = pool };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Pool", (object[] args) => gamesPools[(int)args[0]]).Execute();
        var getObjectStrategy = new GetObject();
        object givenObject = getObjectStrategy.Apply(gameID, objectID);
        Assert.Equal(obj, givenObject);
        var deleteObjectCommand = new DeleteObject(gameID, objectID);
        deleteObjectCommand.Execute();
        Assert.False(pool.ContainsKey(objectID));
    }
    [Fact]
    public void ObjectDidNotExist()
    {
        int gameID = 16;
        int objectID = 128;
        object obj = "object";
        Hashtable pool = new() { [objectID] = obj };
        Hashtable gamesPools = new() { [gameID] = pool };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Pool", (object[] args) => gamesPools[(int)args[0]]).Execute();
        var nonExistentObjectID = 256;
        var getObjectStrategy = new GetObject();
        Assert.Throws<ArgumentNullException>(() => getObjectStrategy.Apply(gameID, nonExistentObjectID));
    }
}
