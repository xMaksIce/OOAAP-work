using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Tests;

public class MacroCommandTest
{
    public MacroCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Macro", 
        (object[] args) => new MacroCommand((IEnumerable<Lib.ICommand>)args[0])).Execute();

        // IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetDependenciesNames", 
        // (object[] args) => ).Execute();
    }
}
