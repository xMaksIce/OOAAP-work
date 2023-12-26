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

public class AssembleMacroCommand
{

}
