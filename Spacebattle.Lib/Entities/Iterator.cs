using Spacebattle.Lib;

public class Iterator: IEnumerator<object>
{   
    int currentPosition = 0;
    List<object> _collection; 
    public Iterator(List<object> collection){
        _collection = collection;
    }
    public object Current => _collection[currentPosition];
    public bool MoveNext() 
    {
      currentPosition++;
      return  currentPosition < _collection.Count(); 
    }
    public void Reset() => currentPosition = 0;
    public void Dispose() => throw new NotImplementedException();
}
