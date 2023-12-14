using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Spacebattle.Lib.Tests;

public class LongOperationTest
{
    [Fact]
    public void RegistersCommandAndQueue_WhenAddingItIntoQueue_ThenCompareTakenAndGivenCommands()
    {
        // making 'IoC.Register' start working
        new InitScopeBasedIoCImplementationCommand().Execute();  

        var moveCommandStartable = new Mock<IMoveCommandStartable>(); 
        // initialize properties for mock object
        moveCommandStartable.SetupGet(mcs => mcs.Target).Returns( new Mock<IUObject>().Object ); 
        moveCommandStartable.SetupGet(mcs => mcs.Property).Returns( new Dictionary<string, object>() );  

        var startMoveCommand = new StartMoveCommand(moveCommandStartable.Object);


        var qMock = new Mock<IQueue>();
        var qReal = new Queue<ICommand>();
        qMock.Setup(q => q.Take()).Returns( () => qReal.Dequeue() );
        qMock.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(
            (ICommand cmd) => {
                qReal.Enqueue(cmd);
            }
        );
        
        var moveCommand = new Mock<ICommand>().Object;

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "Game.Commands.Move", 
            (object[] args)=> {
                return moveCommand;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "Game.Queue", 
            (object[] args)=> {
                return qMock.Object;
            }
        ).Execute();


        startMoveCommand.Execute();
        
        Assert.Equal(moveCommand, qMock.Object.Take());
    }
}
