using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Tests;

public class MessageHandlerTest
{
    [Fact]
    public void CommandAssemblesFromObjThenExecutes()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var command = new Mock<Lib.ICommand>();
        command.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.MessageObjToCommand", (object[] args) => command.Object).Execute();

        var messageObj = new Mock<MessageObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TakeMessageFromQueue", (object[] args) => messageObj.Object).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.PutCommandInQueue", (object[] args) => args[0]).Execute();

        new CreateInterpretation().Execute();
        command.Verify(cmd => cmd.Execute(), Times.Once);
    }
}
