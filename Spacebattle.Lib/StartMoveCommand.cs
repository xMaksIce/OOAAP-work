using Hwdtech;

namespace Spacebattle.Lib;

public class StartMoveCommand: ICommand
{
    private readonly IMoveCommandStartable _moveCommandStartable;

    public StartMoveCommand(IMoveCommandStartable moveCommand) =>  _moveCommandStartable = moveCommand;

    public void Execute()
    {
        _moveCommandStartable.Property.ToList().ForEach(p => _moveCommandStartable.Target.SetProperty(p.Key, p.Value)); // 1. Присвоение свойств.
        
        var cmd = IoC.Resolve<ICommand>("Game.Commands.Move", _moveCommandStartable.Target); // 2. cmd = Конструивание длительной операции (IoC.Resolve<ICommand>("Game.Operation.Move"))

        _moveCommandStartable.Target.SetProperty("Command", cmd); // 3. Закинуть длительную операцию в таргет
        
        IoC.Resolve<IQueue>("Game.Queue").Add(cmd); // 4. cmd закинуть в очередь

    }
}

