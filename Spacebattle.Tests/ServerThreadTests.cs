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

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Create And Start Thread", (object[] args) =>
            {
                return new UActionCommand(
                    () =>
                    {
                        Guid id = (Guid) args[0];

                        var queue = new BlockingCollection<Lib.ICommand>();
                        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                            $"ServerThread.Queue.{id}", (object[] args) =>
                            {
                                return queue;
                            }).Execute();

                        var thread = new ServerThread(queue);
                        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                            $"ServerThread.{id}", (object[] args) =>
                            {
                                return thread;
                            }
                        ).Execute();

                        thread.Start();
                    }
                );
            }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Send Command", (object[] args) =>
            {
                return new UActionCommand(() =>
                    {
                        IoC.Resolve<BlockingCollection<Lib.ICommand>>($"ServerThread.Queue.{(Guid) args[0]}")
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
                    new HardStopCommand((ServerThread) args[0]).Execute();
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
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
        
        var q = IoC.Resolve<BlockingCollection<Lib.ICommand>>($"ServerThread.Queue.{id}"); 
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();
        
        IoC.Resolve<Lib.ICommand>("Send Command", id,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();        

        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        Assert.Single(q);

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {
            
        }

        Assert.True(st.isTerminated());
        cmd.Verify(m => m.Execute(), Times.Once());


    }

    [Fact]
    public void ExceptionCommandShouldNotStopServerThread()
    {
        // AAA
        // Arrange
        var mre = new ManualResetEvent(false);
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();

        var q = IoC.Resolve<BlockingCollection<Lib.ICommand>>($"ServerThread.Queue.{id}"); 

        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        var cmdE = new Mock<Lib.ICommand>();
        cmdE.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })
        ).Execute(); 
             

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
            {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
            })
        ).Execute(); 

        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmdE.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        // Assert
        Assert.Single(q);
        cmdE.Verify(c => c.Execute(), Times.Once);

        cmd.Verify(m => m.Execute(), Times.Exactly(2));
        handleCommand.Verify(m => m.Execute(), Times.Once());


    }

    [Fact]
    public void HardStopCommandExecutesNotInTheThreadItShouldStop()
    {
        var mre = new ManualResetEvent(false);
        Guid id = Guid.NewGuid();
        
        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
        
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        Assert.Throws<Exception>(() =>
            {
                hs.Execute();
            });

        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();
        mre.WaitOne();

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());
    }

    [Fact]
    public void SoftStopCommandShouldStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange
        // new Queue
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();

        var q = IoC.Resolve<BlockingCollection<Lib.ICommand>>($"ServerThread.Queue.{id}"); 
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });

        var ss = IoC.Resolve<Lib.ICommand>("Soft Stop The Thread", hs, st );

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", id,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();        
        
        IoC.Resolve<Lib.ICommand>("Send Command", id, ss).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        Assert.Empty(q);

        cmd.Verify(m => m.Execute(), Times.Exactly(2));

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());

    }
    
    [Fact]
    public void SoftStopCommandExecutesNotInTheThreadItShouldStop()
    {
        var mre = new ManualResetEvent(false); 
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();

        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => {mre.Set();});
        var ss = IoC.Resolve<Lib.ICommand>("Soft Stop The Thread", hs, st);
        
        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();
        

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id,
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

        var cmd = new Mock<Lib.ICommand>();
        cmd.Setup(c => c.Execute());

        IoC.Resolve<Lib.ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());
    }

    [Fact]
    public void DifferentThreadsNotEqual()
    {
        var mre = new ManualResetEvent(false);
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
 
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        Guid anotherSt = Guid.NewGuid();
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => {mre.Set();});

        Assert.False(st.Equals(anotherSt));

        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());

    }

    [Fact]
    public void DifferentThreadsHaveDiffHashCode()
    {
        var mre = new ManualResetEvent(false); 
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");
        
        var anotherThreadHashCode = 0;
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => {mre.Set();});

        Assert.True(st.GetHashCode() != anotherThreadHashCode);

        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();
        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());
    }

    [Fact]
    public void SameThreadsEqual()
    {
        var mre = new ManualResetEvent(false);
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
        
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");
        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => {mre.Set();});
        
        var checkCmd = new UActionCommand ( () => {
            Assert.False(st.Equals(Thread.CurrentThread));
        });

        var handleCommand = new Mock<Lib.ICommand>();
        handleCommand.Setup(m => m.Execute()).Verifiable();

        IoC.Resolve<Lib.ICommand>("Send Command", id, 
            new UActionCommand(() =>
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                        IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                })).Execute();
        IoC.Resolve<Lib.ICommand>("Send Command", id,
                new UActionCommand(() =>
                {
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Exception.Handle", (object[] args) => handleCommand.Object).Execute();
                })
            ).Execute();

        IoC.Resolve<Lib.ICommand>("Send Command", id, checkCmd).Execute();

        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();
        
        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());
    }

    [Fact]
    public void NullNotEqualCurrentThread()
    {
        var mre = new ManualResetEvent(false);
        Guid id = Guid.NewGuid();

        IoC.Resolve<Lib.ICommand>("Create And Start Thread", id).Execute();
        var st = IoC.Resolve<ServerThread>($"ServerThread.{id}");

        var hs = IoC.Resolve<Lib.ICommand>("Hard Stop The Thread", st, () => {mre.Set();});
        
        Assert.False(st.Equals(null));

        IoC.Resolve<Lib.ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();

        using (var thread = IoC.Resolve<ServerThread>($"ServerThread.{id}"))
        {

        }
        Assert.True(st.isTerminated());
    }
}
