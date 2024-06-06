namespace Spacebattle.Lib;
public interface IStrategy
{
    public object Apply(params object[] args);
}
