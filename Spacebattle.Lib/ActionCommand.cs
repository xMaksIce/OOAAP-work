namespace Spacebattle.Lib;

public class ActionCommand : ICommand
{
    private readonly Action<object[]> _function;
    private readonly object[] _args;

    public ActionCommand(Action<object[]> function, object[] args)
    {
        _function = function;
        _args = args;
    }

    public void Execute()
    {
        _function(_args);
    }
}
