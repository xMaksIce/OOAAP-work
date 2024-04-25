using Hwdtech;

namespace Spacebattle.Lib;

public class SoftStopCommand: ICommand
{
    private readonly UActionCommand _hsc;
    private readonly ServerThread _st;
    readonly Action softStopStrategy;
    public SoftStopCommand(UActionCommand hsc, ServerThread st) 
    {
        _hsc = hsc;
        _st = st;

        softStopStrategy = () => 
        {
            if (!_st._q.TryTake(out var cmd))
            {   
                _hsc.Execute();
            }
            try 
            {   
                cmd!.Execute();
            }
            catch (Exception e) 
            {
                IoC.Resolve<ICommand>("Exception.Handle", cmd!, e).Execute();
            }
            
        };
    } 
    public void Execute() 
    {
        if (_st.Equals(Thread.CurrentThread)) 
            {
                _st.UpdateStrategy(softStopStrategy);
            } 
        else 
            {  
                throw new Exception("WRONG THREAD!");
            }

    }
}
