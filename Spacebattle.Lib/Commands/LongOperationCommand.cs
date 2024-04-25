using Hwdtech;

namespace Spacebattle.Lib;
public class LongOperationCommand : ICommand
{
    private readonly IMoveCommandStartable _moveCommandStartable;
    private readonly string _dependency;


    public LongOperationCommand(string dependency, IMoveCommandStartable mcs)
    {
        _dependency = dependency;
        _moveCommandStartable = mcs;
    }

    public void Execute()
    {
        _moveCommandStartable.Property.ToList().ForEach(p => _moveCommandStartable.Target.SetProperty(p.Key, p.Value));

        string depPath = string.Format("Game.Commands.{0}", _dependency);

        var cmd = IoC.Resolve<ICommand>(depPath, _moveCommandStartable.Target);

        var macro = IoC.Resolve<ICommand>("Game.Commands.MacroCommand", cmd);

        ICommand injectableCommand = IoC.Resolve<ICommand>("Game.Commands.Injectable", cmd);

        _moveCommandStartable.Target.SetProperty("InjectableCommand", injectableCommand);

        IoC.Resolve<IQueue>("Game.Queue").Add(cmd);

    }
}
