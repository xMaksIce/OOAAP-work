using Hwdtech;

namespace Spacebattle.Lib;
public interface IMoveEndable
{
    public BridgeCommand Command { get; }
    public UObject Object { get; }
    public IEnumerable<object> Property { get; }
}

public class EndMoveCommand
{
    private IMoveEndable endableCommand;
    public EndMoveCommand(IMoveEndable command){ endableCommand = command; }
    public void Execute()
    {
        IoC.Resolve<string>("Game.Object.DeleteProperty", endableCommand.Object, endableCommand.Property);
        var dummyCommand = IoC.Resolve<ICommand>("Game.Command.DummyCommand");
        IoC.Resolve<IInjectable>("Game.Command.Inject", endableCommand.Command, dummyCommand);
    }
}
