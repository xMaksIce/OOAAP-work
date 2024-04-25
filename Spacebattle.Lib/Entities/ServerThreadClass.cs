using System.Collections.Concurrent;
using Hwdtech;

namespace Spacebattle.Lib;


public class ServerThread : IDisposable
{
    private Thread _t;
    internal readonly BlockingCollection<ICommand> _q;
    private bool stop = false;
    private Action strategy;
    private Action defaultStrategy;
    Action afterThread = () => { };

    public ServerThread(BlockingCollection<ICommand> q, Action beforeThread, Action? afterThread = null)
    {
        _q = q;
        defaultStrategy = () =>
        {
            var cmd = _q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
            }

        };

        strategy = defaultStrategy;

        if (afterThread != null) this.afterThread = afterThread;

        _t = new Thread(() =>
        {
            beforeThread();

            while (!stop)
            {
                strategy();
            }

            this.afterThread();
        });

    }
    public bool isTerminated()
    {
        return !_t.IsAlive;
    }

    public void Dispose()
    {
        _t.Join();
    }
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() == typeof(Thread) || obj.GetType() == typeof(ServerThread))
        {
            return _t == obj;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void Start()
    {
        _t.Start();
    }

    internal void Stop()
    {
        stop = true;

    }
    internal void UpdateStrategy(Action newStrat)
    {
        strategy = newStrat;
    }
}
