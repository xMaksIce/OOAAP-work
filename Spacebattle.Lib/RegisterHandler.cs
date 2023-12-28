using Hwdtech;

namespace Spacebattle.Lib;

public interface IHandler
{
    public void Handle();
}

public class RegisterExceptionHandler : ICommand
{
    private readonly ICommand command;
    private readonly Exception exception;
    private readonly IHandler handler;
    public RegisterExceptionHandler(ICommand command, Exception exception, IHandler handler) 
    {
        this.command = command;
        this.exception = exception;
        this.handler = handler;
    }
    public void Execute()
    {
        Type typeCmd = command.GetType();
        Type typeExc = exception.GetType();
        IoC.Resolve<ICommand>("Game.Handle.Register", typeCmd, typeExc, handler);
    }
}
