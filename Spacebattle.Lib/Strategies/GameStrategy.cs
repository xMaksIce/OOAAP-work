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
public class CreateScope : IStrategy
{
    public object Apply(params object[] args)
    {
        object parentScope = args[0];
        object scope = IoC.Resolve<object>("Scopes.New", parentScope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetTickValue", (object[] args) => new GetTick().Apply(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TakeFromQueue", (object[] args) => new TakeFromQueue().Apply(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.PutInQueue", (object[] args) => new PutInQueue((int)args[0], (ICommand)args[1])).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetObject", (object[] args) => new GetObject().Apply(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DeleteObject", (object[] args) => new DeleteObject((int)args[0], (int)args[1])).Execute();
        return scope;
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
public class GetTick : IStrategy
{
    public object Apply(params object[] args)
    {
        int gameID = (int)args[0];
        int tick = IoC.Resolve<int>("Game.Tick", gameID);
        return tick;
    }
}
