using System.Collections;
using Hwdtech;
namespace Spacebattle.Lib;
public class InitializeGameplayStrategies : ICommand
{
    readonly private Hashtable _strategies;
    public InitializeGameplayStrategies(Hashtable strategies) { _strategies = strategies; }
    public void Execute()
    {
        var keyStrategy = _strategies.Keys.Cast<string>().Zip(
            _strategies.Values.Cast<IStrategy>(), (key, strategy) => new { Key = key, Value = strategy }).ToList();
        keyStrategy.ForEach(kv => IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", "Game.Command." + kv.Key, (object[] args) => kv.Value.Apply(args)).Execute());
    }
}
public interface IMessageObject
{
    public string Type { get; }
    public int GameID { get; }
    public int ItemID { get; }
    public Hashtable Parameters { get; }
}
public class InterpretCommand : ICommand
{
    private readonly IMessageObject _messageObj;
    public InterpretCommand(IMessageObject messageObj)
    {
        _messageObj = messageObj;
    }
    public void Execute()
    {
        var gameObject = IoC.Resolve<object>("Game.Object", _messageObj.GameID, _messageObj.ItemID);
        var command = IoC.Resolve<ICommand>("Game.Command." + _messageObj.Type, gameObject, _messageObj.Parameters);
        IoC.Resolve<ICommand>("Game.PutCommandInQueue", command, _messageObj.GameID).Execute();
    }
}
public class CreateStartMove : IStrategy
{
    public object Apply(params object[] args)
    {
        var moveStartable = IoC.Resolve<IMoveCommandStartable>("Game.Object.Startable", args[0]);
        var startCommand = new StartMoveCommand(moveStartable);
        return startCommand;
    }
}
public class CreateEndMove : IStrategy
{
    public object Apply(params object[] args)
    {
        var moveEndable = IoC.Resolve<IMoveEndable>("Game.Object.Endable", args[0]);
        var endCommand = new EndMoveCommand(moveEndable);
        return endCommand;
    }
}
public class CreateRotate : IStrategy
{
    public object Apply(params object[] args)
    {
        var rotatable = IoC.Resolve<IRotatable>("Game.Object.Rotatable", args[0]);
        var rotateCommand = new RotateCommand(rotatable);
        return rotateCommand;
    }
}
public class CreateShoot : IStrategy
{
    public object Apply(params object[] args)
    {
        var shootable = IoC.Resolve<IShootable>("Game.Object.Shootable", args[0]);
        var shootCommand = new ShootCommand(shootable);
        return shootCommand;
    }
}
