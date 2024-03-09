namespace Spacebattle.Lib;

public class SoftStopCommand: ICommand
{
    private readonly HardStopCommand _hsc;
    private ServerThread _sv;
    public SoftStopCommand(HardStopCommand hsc, ServerThread sv) 
    {
        _hsc = hsc;
        _sv = sv;
    } 
    public void Execute() 
    {
        // queue executes till the very last
        if (!_sv._q.TryTake(out var cmd)) return; 

        _hsc.Execute();

        // ServerThread.Stop();
    }

}
