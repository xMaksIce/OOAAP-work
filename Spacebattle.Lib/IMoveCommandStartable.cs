namespace Spacebattle.Lib;

public interface IMoveCommandStartable
{
    public IUObject Target { get;}
    public Dictionary<string, object> Property { get;}
    
}