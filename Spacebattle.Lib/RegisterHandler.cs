using System.Collections;
using Hwdtech;

namespace Spacebattle.Lib;

public class RegisterExceptionHandler : ICommand
{
    private readonly object cmdOrExc;
    private readonly ICommand handler;
    public RegisterExceptionHandler(object cmdOrExc, ICommand handler) 
    {
        this.cmdOrExc = cmdOrExc;
        this.handler = handler;
    }

    public void Execute()
    {
        Type objType = cmdOrExc.GetType();
        
        var tree = IoC.Resolve<Hashtable>("Game.Handle.GetTree");

        if (!tree.ContainsKey(objType))
        {
            tree[objType] = handler;
        }
    }
}
