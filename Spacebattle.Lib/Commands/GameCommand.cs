using Hwdtech;

namespace Spacebattle.Lib;
public class GameCommand : ICommand
{
    readonly object _scope;
    readonly Queue<ICommand> _q;
    public GameCommand(object scope, Queue<ICommand> q)
    {
        _scope = scope;
        _q = q;
    }
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", _scope)
        ).Execute();
        var timeQuantum = (int) IoC.Resolve<object>("GameCommand.TimeQuantum");
        var time = System.Diagnostics.Stopwatch.StartNew();
        
        while (time.ElapsedMilliseconds < timeQuantum)
        {
            if (!_q.TryDequeue(out var cmd)) return; 
            try
            {
                cmd.Execute();
            }
            catch(Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handle", e, cmd).Execute();
            }
        }
    }
}
