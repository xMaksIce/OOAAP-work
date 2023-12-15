namespace Spacebattle.Lib;
public interface UObject
{
    public object GetProperty(string key);
    public void SetProperty(string key, object value);
    public void DeleteProperty(string key);
}
