namespace Spacebattle.Lib;

public interface IVectorslike
{
    public List<List<int>> ToActualVectors();
}

public class BuildTree: ICommand
{
    private readonly IVectorslike _rawVectors;
    public BuildTree(IVectorslike vectorslike)
    {
        _rawVectors = vectorslike;
    }
    public void Execute()
    {

    }
}
