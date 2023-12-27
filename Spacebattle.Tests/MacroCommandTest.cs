using System.Collections;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Tests;

public class MacroCommandTest
{
    [Fact]
    public void MacroTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Macro", 
        (object[] args) => new MacroCommand((IEnumerable<Lib.ICommand>)args[0])).Execute();
        
        Hashtable operationToDepsNames = new(){
            {"MyDoubleCommandDependency", new List<string>{"Game.Command.First", "Game.Command.Second"}}};
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetDependenciesNames", 
        (object[] args) => operationToDepsNames[args[0]]).Execute();

        Mock<object> target = new Mock<object>();
        Mock<Lib.ICommand> firstCmd = new();
        Mock<Lib.ICommand> secondCmd = new();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.First",
        (object[] args) => firstCmd.Object).Execute(); 
        // target по факту не используется, в реальности было бы например: (args) => new MoveCommand(args[0])
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Second",
        (object[] args) => secondCmd.Object).Execute();

        MacroCommand macroCommand = BuldingMacroCommandStrategy.Build("MyDoubleCommandDependency", target.Object);
        macroCommand.Execute();
        firstCmd.Verify(cmd => cmd.Execute(), Times.Once);
        secondCmd.Verify(cmd => cmd.Execute(), Times.Once);
    }
}
