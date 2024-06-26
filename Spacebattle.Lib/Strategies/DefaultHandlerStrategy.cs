namespace Spacebattle.Lib;

public class DefaultHandlerStrategy : IStrategy
{
    public object Run(object[] args)
    {
        throw (Exception)args[0];
    }
}
