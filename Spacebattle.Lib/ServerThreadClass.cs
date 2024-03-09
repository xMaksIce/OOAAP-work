using System.Collections.Concurrent;
using System.Data.Common;
using Hwdtech;

namespace Spacebattle.Lib;


class ThreadStrategy
{
    BlockingCollection<ICommand> q = new BlockingCollection<ICommand>(100);

    Thread t = new Thread( () => 
    {
        while(!stop) {
            var cmd = q.Take();
            cmd.Execute();

        }
    });

    t .start();
}

public class ServerThread 
{
    private Thread _t;
    // private readonly int _id;
    internal readonly BlockingCollection<ICommand> _q;
    private bool stop = false;
    Action strategy, defaultStrategy;
    Action softStopStrategy;
    Action hardStopStrategy;

    public ServerThread(BlockingCollection<ICommand> q) 
    {
        _q = q;

        // _id = id;
        defaultStrategy = ()=> {
            var cmd = q.Take();
            
            try 
            {
                cmd.Execute();
            }
            catch (Exception e) 
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
            }

        };

        strategy = defaulStrategy;

        _t = new Thread(()=> {
            while(!stop) 
            {
                strategy(); 
                
            }
        });

        Action hardStopStrategy = () =>  
        {
            SameThreadCheck(_id);
            HardStopCommand(this.ServerThread);

        };

        Action softStopStrategy = () => 
        {
        
        while (!stop)
        {
            if (!_q.TryTake(out var cmd))
            {
                UpdateStrategy(hardStopStrategy);
                    // strategy = ;
                return;
            }
                // var cmd = _q.Take();
            try 
            {
                cmd.Execute();
            }
            catch (Exception e) 
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
            }
        }
            
        };

        
    }
    Action defaulStrategy = () => 
    {       var cmd = _q.Take();
            try 
            {
                cmd.Execute();
            }
            catch (Exception e) 
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd, e).Execute();
            }
    };
    
     // override object.Equals
    // public override bool Equals(object obj)
    // {
    //     if (obj.GetType() == typeof(Thread))
    //     {
    //         return _t == (Thread)obj;
    //     }

    //     if (obj == null || GetType() != obj.GetType())
    //     {
    //         return false;
    //     }

    //     return false;
    // }

    // // override object.GetHashCode
    // public override int GetHashCode()
    // {
    //     // TODO: write your implementation of GetHashCode() here
    //     // throw new System.NotImplementedException();
    //     return base.GetHashCode();
    // }
    
    public void Start() {
        _t.Start();
    }

    internal void  Stop() { 
        stop = true;
        
    }
    // Thread.CurrentThread
    // void SameThreadCheck(int id)
    // {
    //     if (id == _id) return;
    //     throw new Exception();
    // }
    void SameThreadCheck(Thread thr)
    {
        if (_t == thr) return;
        throw new Exception();
    }

    internal void UpdateStrategy(Action newStrat) {
        strategy = newStrat;
    }
}
