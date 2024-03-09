namespace Spacebattle.Lib;

public class HardStopCommand : ICommand
{
    private ServerThread _t;
    public HardStopCommand(ServerThread t) {
        _t = t;
    }
    public void Execute()
    {
        if (_t.Equals(Thread.CurrentThread)) 
        {
            _t.Stop();
        } 
        else 
        {
            throw new Exception("WRONG!");
        }
    }
}
// public class HardStopCommand: ICommand 
// {
//     // private readonly BlockingCollection<ICommand> _bc;

//     public HardStopCommand(ServerThread serverThread)
//     {
//         _serverThread = serverThread;
//     }

//     private readonly ServerThread _serverThread;

//     public void Execute()
//     {
//         _serverThread.Stop();
//         // _bc.stop();
//     }
// }
