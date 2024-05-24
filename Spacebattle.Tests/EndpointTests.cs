using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Spacebattle.Server;

namespace Spacebattle.Tests;
public class EndpointTests
{
    public EndpointTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

    }
    [Fact]
    public void SuccessfullyFoundThreadByGameId_SentMessage()
    {
        var contract1 = new SpacebattleContract()
        {
            CommandType = "fire",
            GameId = "game1",
            GameItemId = 2,
            GameParameters = new Dictionary<string, object>() {
                {"double_damage", true}
            }
        };
        var contract2 = new SpacebattleContract()
        {
            CommandType = "add fast fuel",
            GameId = "game1",
            GameItemId = 3,
            GameParameters = new Dictionary<string, object>()
        };
        var endpoint = new WebApi();

        var threadId = "1";
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetThreadId",
            (object[] args) => threadId
        ).Execute();

        var cmdFromMessage = new Mock<Lib.ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetCommandFromMessage",
            (object[] args) => cmdFromMessage.Object
        ).Execute();

        var cmdFinal = new Mock<Lib.ICommand>();
        cmdFinal.Setup(c => c.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.SendMessageQueue",
            (object[] args) => cmdFinal.Object
        ).Execute();

        endpoint.SendMessage(contract1);
        endpoint.SendMessage(contract2);

        cmdFinal.Verify(c => c.Execute(), Times.Exactly(2));
    }

    [Fact]
    public void FailedToFindThreadId()
    {
        var contract1 = new SpacebattleContract()
        {
            CommandType = "fire",
            GameId = "id2",
            GameItemId = 2,
            GameParameters = new Dictionary<string, object>() {
                {"double_damage", true}
            }
        };

        var endpoint = new WebApi();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetThreadId",
            (object[] args) =>
            {
                return (object)new Exception();
            }
        ).Execute();

        var cmdFromMessage = new Mock<Lib.ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetCommandFromMessage",
            (object[] args) => cmdFromMessage.Object
        ).Execute();

        var cmdFinal = new Mock<Lib.ICommand>();
        cmdFinal.Setup(c => c.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.SendMessageQueue",
            (object[] args) => cmdFinal.Object
        ).Execute();

        Assert.Throws<InvalidCastException>(() => endpoint.SendMessage(contract1));

    }

    [Fact]
    public void FailedToExecuteQueueFinalCommand()
    {
        var contract = new SpacebattleContract()
        {
            CommandType = "fire",
            GameId = "3",
            GameItemId = 2,
            GameParameters = new Dictionary<string, object>() {
                {"double_damage", true}
            }
        };

        var endpoint = new WebApi();

        var threadId = "1";
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetThreadId",
            (object[] args) =>
            {
                return threadId;
            }
        ).Execute();

        var cmdFromMessage = new Mock<Lib.ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetCommandFromMessage",
            (object[] args) => cmdFromMessage.Object
        ).Execute();

        var cmdFinal = new Mock<Lib.ICommand>();
        cmdFinal.Setup(c => c.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.SendMessageQueue",
            (object[] args) => cmdFinal.Object
        ).Execute();

        Assert.Throws<Exception>(() => endpoint.SendMessage(contract));

    }

    [Fact]
    public void FailedToReceiveMessageCommand()
    {
        var contract = new SpacebattleContract()
        {
            CommandType = "fire",
            GameId = "3",
            GameItemId = 2,
            GameParameters = new Dictionary<string, object>() {
                {"double_damage", true}
            }
        };

        var endpoint = new WebApi();

        var threadId = "1";
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetThreadId",
            (object[] args) =>
            {
                return threadId;
            }
        ).Execute();

        var exCmd = new Mock<Lib.ICommand>();
        exCmd.Setup(c => c.Execute()).Throws(new Exception());
        var cmdFromMessage = new Mock<Lib.ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.GetCommandFromMessage",
            (object[] args) =>
            {
                exCmd.Object.Execute();
                return cmdFromMessage.Object;
            }
        ).Execute();

        var cmdFinal = new Mock<Lib.ICommand>();
        cmdFinal.Setup(c => c.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread.SendMessageQueue",
            (object[] args) => cmdFinal.Object
        ).Execute();

        Assert.Throws<Exception>(() => endpoint.SendMessage(contract));

    }
}
