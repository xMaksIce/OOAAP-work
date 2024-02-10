namespace Spacebattle.Lib;
public interface IInjectable
{
    void Inject(ICommand obj);
}
public class BridgeCommand : ICommand, IInjectable
{
    private ICommand internalCommand;
    public BridgeCommand(ICommand command)
    {
        internalCommand = command;
    }
    public void Inject(ICommand command)
    {
        internalCommand = command;
    }
    public void Execute()
    {
        internalCommand.Execute();
    }
}
