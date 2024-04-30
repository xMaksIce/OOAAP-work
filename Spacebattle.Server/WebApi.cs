using Hwdtech;
using CoreWCF;

namespace Spacebattle.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WebApi : IWebApi
    {
        public void SendMessage(SpacebattleContract contract)
        {

            var threadId = IoC.Resolve<string>("ServerThread.GetThreadId", contract.GameId);

            var cmdFromMessage = IoC.Resolve<Lib.ICommand>("ServerThread.GetCommandFromMessage", contract);

            IoC.Resolve<Lib.ICommand>($"ServerThread.SendMessageQueue{threadId}", contract, cmdFromMessage).Execute();
        }
    }
}
