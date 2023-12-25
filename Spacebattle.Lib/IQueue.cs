namespace Spacebattle.Lib;

public interface IQueue 
{
    void Add(ICommand cmd);
    ICommand Take();
}
