using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;



namespace Spacebattle.Tests;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var thread = new ConcurrentDictionary<int, ServerThread>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "ServerThread", (object[] args) =>
            {
                return thread;
            }
        ).Execute();

        var queue = new ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "ServerThread.Queue", (object[] args) =>
            {
                return queue;
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Create And Start Thread", (object[] args) =>
            {
                return new UActionCommand(
                    () =>
                    {
                        var realQueue = new BlockingCollection<Lib.ICommand>();   // new BlockingCollection<Lib.ICommand>();
                        var realThread = new ServerThread(realQueue);
                        
                        realThread.Start();

                        IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("ServerThread.Queue").TryAdd((int) args[0], realQueue);
                        IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread").TryAdd((int) args[0], realThread);
                    }
                );
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Send Command", (object[] args) =>
            {
                return new UActionCommand(() =>
                    {
                        IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("ServerThread.Queue")[(int) args[0]]
                            .Add((Lib.ICommand) args[1]);
                    }
                );
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Hard Stop The Thread",
            (object[] args) =>
            {
                return new UActionCommand(
                () =>
                {
                    new HardStopCommand(
                        (ServerThread) args[0]
                        ).Execute();
                    if (args.Length == 2)
                        new UActionCommand((Action)args[1]).Execute();
                });
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", 
            "Soft Stop The Thread",
                (object[] args) => 
                {
                    return new UActionCommand(
                        () => 
                        {
                            new SoftStopCommand((UActionCommand) args[0], (ServerThread) args[1]).Execute();
                            
                            if (args.Length == 3) 
                            new UActionCommand((Action) args[2]).Execute();
                        }
                    );
                }
        ).Execute();

    }

    [Fact]
    public void HardStopCommandShouldStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 0).Execute();

        var q = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("ServerThread.Queue")[0];  //new BlockingCollection<Lib.ICommand>(10);
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[0];

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        IoC.Resolve<Lib.ICommand>("Send Command", 0, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", 0,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();        

        IoC.Resolve<Lib.ICommand>("Send Command", 0, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 0, hs).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 0, cmd.Object).Execute();

        mre.WaitOne();

        Assert.Single(q);

        cmd.Verify(m => m.Execute(), Times.Once());
    }

    [Fact]
    public void ExceptionCommandShouldNotStopServerThread()
    {
        // AAA
        // Arrange
        var mre = new ManualResetEvent(false);
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 1).Execute();

        var q = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("ServerThread.Queue")[1]; // new BlockingCollection<Lib.ICommand>(10);
        
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[1];

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        var cmdE = new Mock<Lib.ICommand>();
        cmdE.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", 1, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })
        ).Execute(); 
             

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", 1, 
            new UActionCommand(() =>
            {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
            })
        ).Execute(); 

        IoC.Resolve<Lib.ICommand>("Send Command", 1, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 1, cmdE.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 1, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 1, hs).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 1, cmd.Object).Execute();

        mre.WaitOne();

        // Assert
        Assert.Single(q);
        cmdE.Verify(c => c.Execute(), Times.Once);

        cmd.Verify(m => m.Execute(), Times.Exactly(2));
        handleCommand.Verify(m => m.Execute(), Times.Once());

    }

    // // throws exception (executes NOT in the thread it's aboutta stop)
    [Fact]
    public void HardStopCommandExecutesNotInTheThreadItShouldStop()
    {
        
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 2).Execute();

        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[2];
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);

        Assert.Throws<Exception>(() =>
            {
                hs.Execute();
            });

        IoC.Resolve<Lib.ICommand>("Send Command", 2, hs).Execute();
    }

    [Fact]
    public void SoftStopCommandShouldStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange
        // new Queue

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 3).Execute();

        var q = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("ServerThread.Queue")[3];  //new BlockingCollection<Lib.ICommand>(10);
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[3];

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        var ss = IoC.Resolve<Lib.ICommand>("Soft Stop The Thread", hs, st);

        IoC.Resolve<Lib.ICommand>("Send Command", 3, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", 3,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();        
        
        IoC.Resolve<Lib.ICommand>("Send Command", 3, ss).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 3, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 3, cmd.Object).Execute();

        mre.WaitOne();

        Assert.Empty(q);

        cmd.Verify(m => m.Execute(), Times.Exactly(2));
    }
    
    [Fact]
    public void SoftStopCommandExecutesNotInTheThreadItShouldStop()
    {
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 4).Execute();

        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[4];
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);
        var ss = IoC.Resolve<Lib.ICommand>("Soft Stop The Thread", hs, st);
        
        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();
        

        IoC.Resolve<Lib.ICommand>("Send Command", 4, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", 4,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();   

        Assert.Throws<Exception>(() =>
            {
                ss.Execute();
            });

        IoC.Resolve<Lib.ICommand>("Send Command", 4, ss).Execute();
    }

    [Fact]
    public void DifferentThreadsNotEqual()
    {
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 5).Execute();
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[5];
        var anotherSt = 6;
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);

        Assert.False(st.Equals(anotherSt));

        IoC.Resolve<Lib.ICommand>("Send Command", 5, hs).Execute();
    }

    [Fact]
    public void DifferentThreadsHaveDiffHashCode()
    {
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 6).Execute();
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[6];
        var anotherThreadHashCode = 0;
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);

        Assert.True(st.GetHashCode() != anotherThreadHashCode);

        IoC.Resolve<Lib.ICommand>("Send Command", 6, hs).Execute();
    }

    [Fact]
    public void SameThreadsEqual()
    {
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 7).Execute();
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[7];
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);
        
        var checkCmd = new UActionCommand ( () => {
            Assert.True(st.Equals(Thread.CurrentThread));
        });

        IoC.Resolve<Lib.ICommand>("Send Command", 7, checkCmd).Execute();

        IoC.Resolve<Lib.ICommand>("Send Command", 7, hs).Execute();
    }

    [Fact]
    public void NullNotEqualCurrentThread()
    {
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", 8).Execute();
        var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread")[8];
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st);
        
        Assert.False(st.Equals(null));

        IoC.Resolve<Lib.ICommand>("Send Command", 8, hs).Execute();
    }
}
