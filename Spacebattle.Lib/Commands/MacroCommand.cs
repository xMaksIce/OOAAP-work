using Hwdtech;

namespace Spacebattle.Lib;

public class MacroCommand : ICommand
{
    private readonly IEnumerable<ICommand> commandSet;
    public MacroCommand(IEnumerable<ICommand> commandsSet) { this.commandSet = commandsSet; }
    public void Execute()
    {
        commandSet.ToList().ForEach(command => command.Execute());
    }
}

public class BuldingMacroCommandStrategy
{
    public static MacroCommand Build(string operationName, object target)
    {
        List<ICommand> commands = new();
        IEnumerable<string> dependenciesNames = IoC.Resolve<IEnumerable<string>>("GetDependenciesNames", operationName);
        dependenciesNames.ToList().ForEach(dependencyName => commands.Add(IoC.Resolve<ICommand>(dependencyName, target)));
        return IoC.Resolve<MacroCommand>("Game.Command.Macro", commands);
    }
}
