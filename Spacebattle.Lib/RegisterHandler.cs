using System.Collections;
using Hwdtech;

namespace Spacebattle.Lib;

public class RegisterExceptionHandler : ICommand
{
    private readonly ICommand command;
    private readonly Exception exception;
    private readonly ICommand handler;
    public RegisterExceptionHandler(ICommand command, Exception exception, ICommand handler) 
    {
        this.command = command;
        this.exception = exception;
        this.handler = handler;
    }
    public void Execute()
    {
        var typeCmd = command.GetType();
        var typeExc = exception.GetType();
        
        Hashtable? tree = IoC.Resolve<Hashtable>("Game.Handle.GetTree");

        if (!tree.ContainsKey(typeCmd)) tree.Add(typeCmd, new Hashtable());
        tree = (Hashtable?) tree[typeCmd] ?? new Hashtable();
        if (!tree.ContainsKey(typeExc)) tree.Add(typeExc, handler);
    }
}
