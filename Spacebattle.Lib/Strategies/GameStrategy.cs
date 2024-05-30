using System.Collections;
using Hwdtech;
namespace Spacebattle.Lib;
public interface IStrategy
{
    public object Apply(params object[] args);
}
public class GameCommand : ICommand
{
    readonly private int _gameTick;
    readonly private object _scope;
    readonly private Queue<ICommand> _queue;
    public GameCommand(int gameTick, object scope, Queue<ICommand> queue)
    {
        _gameTick = gameTick;
        _scope = scope;
        _queue = queue;
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>("GameCommandImplementation", _gameTick, _scope, _queue).Execute();
    }
}
public class CreateGame : IStrategy
{
    public object Apply(params object[] args)
    {
        var tick = (int)args[0];
        var scope = args[1];
        var queue = new Queue<ICommand>();
        var gameCommand = new GameCommand(tick, scope, queue);
        return gameCommand;
    }
}
public class DeleteGame : ICommand
{
    readonly private int _gameID;
    public DeleteGame(int gameID)
    {
        _gameID = gameID;
    }
    public void Execute()
    {
        var table = IoC.Resolve<Hashtable>("Game.Scopes.List");
        table.Remove(_gameID);
    }
}
public class TakeFromQueue : IStrategy
{
    public object Apply(params object[] args)
    {
        int gameID = (int)args[0];
        var queue = IoC.Resolve<Queue<ICommand>>("Game.Queue", gameID);
        ICommand command = queue.Dequeue();
        return command;
    }
}
public class PutInQueue : ICommand
{
    readonly int _gameID;
    readonly ICommand _command;
    public PutInQueue(int gameID, ICommand command)
    {
        _gameID = gameID;
        _command = command;
    }
    public void Execute()
    {
        var queue = IoC.Resolve<Queue<ICommand>>("Game.Queue", _gameID);
        queue.Enqueue(_command);
    }
}
public class GetObject : IStrategy
{
    public object Apply(params object[] args)
    {
        int gameID = (int)args[0];
        int objectID = (int)args[1];
        var pool = IoC.Resolve<Hashtable>("Game.Pool", gameID);
        object obj = pool[objectID] ?? throw new ArgumentNullException();
        return obj;
    }
}
public class DeleteObject : ICommand
{
    private readonly int _gameID;
    private readonly int _objectID;
    public DeleteObject(int gameID, int objectID)
    {
        _gameID = gameID;
        _objectID = objectID;
    }
    public void Execute()
    {
        var pool = IoC.Resolve<Hashtable>("Game.Pool", _gameID);
        pool.Remove(_objectID);
    }
}
