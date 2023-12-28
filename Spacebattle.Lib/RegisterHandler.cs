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
        
        var tree = IoC.Resolve<Dictionary<Type, Dictionary<Type, ICommand>>>("Game.Handle.GetTree");

        tree.TryAdd(typeCmd, new Dictionary<Type, ICommand>());
        tree[typeCmd].TryAdd(typeExc, handler);
    }
}
