using Hwdtech;

namespace Spacebattle.Lib;
public class StartMacroCommand : ICommand
{
    private readonly IMoveCommandStartable _moveCommandStartable;
    private readonly string _dependency;
    

    public StartMacroCommand(string dependency,  IMoveCommandStartable mcs)
    {
        _dependency = dependency;
        _moveCommandStartable = mcs;
    }

    public void Execute()
    {
        _moveCommandStartable.Property.ToList().ForEach(p => _moveCommandStartable.Target.SetProperty(p.Key, p.Value));
            
        string depPath = string.Format("Game.Commands.{0}", _dependency);

        var cmd = IoC.Resolve<ICommand>(depPath, _moveCommandStartable.Target);
        
        // var macro = IoC.Resolve<ICommand>("Game.Commands.MacroCommand", cmd);

        ICommand injectableCommand = IoC.Resolve<ICommand>("Game.Commands.Injectable", cmd); 

        _moveCommandStartable.Target.SetProperty("InjectableCommand", injectableCommand); 

        IoC.Resolve<IQueue>("Game.Queue").Add(cmd); 


    }
}

// _moveCommandStartable.Property.ToList().ForEach(p => _moveCommandStartable.Target.SetProperty(p.Key, p.Value)); // 1. Присвоение свойств.

// var cmd = IoC.Resolve<ICommand>("Game.Commands.Move", _moveCommandStartable.Target); // 2. cmd = Конструивание длительной операции (IoC.Resolve<ICommand>("Game.Operation.Move"))

// var injectableCommand = IoC.Resolve<ICommand>("Game.Commands.Injectable", cmd); // inject - обёртка 

// _moveCommandStartable.Target.SetProperty("InjectableCommand", injectableCommand); // 3. Закинуть длительную операцию в таргет

// IoC.Resolve<IQueue>("Game.Queue").Add(cmd); // 4. cmd закинуть в очередь