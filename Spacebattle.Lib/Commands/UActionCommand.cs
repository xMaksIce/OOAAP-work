namespace Spacebattle.Lib;
public class UActionCommand : ICommand
{
    private readonly Action _action;
    public UActionCommand(Action action)
    {
        _action = action;
    }
    public void Execute()
    {
        _action();
    }
}
