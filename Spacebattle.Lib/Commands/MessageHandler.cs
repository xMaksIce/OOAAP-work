using System.Collections;
using Hwdtech;

namespace Spacebattle.Lib;

public interface MessageObject
{
    public string Type { get; }
    public int GameID { get; }
    public int ItemID { get; }
    public Hashtable Parameters { get; }
}
public class InterpretMessage : ICommand
{
    private readonly MessageObject _messageObj;
    public InterpretMessage(MessageObject messageObj)
    {
        _messageObj = messageObj;
    }
    public void Execute()
    {
        var command = IoC.Resolve<ICommand>("Game.MessageObjToCommand", _messageObj);
        IoC.Resolve<ICommand>("Game.PutCommandInQueue", command, _messageObj.GameID).Execute();
    }
}
public class CreateInterpretation : ICommand
{
    private readonly int _queueID;
    public CreateInterpretation(int queueID)
    {
        _queueID = queueID;
    }
    public void Execute()
    {
        var messageObject = IoC.Resolve<MessageObject>("Game.TakeMessageFromQueue", _queueID);
        var interpretation = new InterpretMessage(messageObject);
        IoC.Resolve<ICommand>("Game.PutCommandInQueue", interpretation, messageObject.GameID).Execute();
    }
}
