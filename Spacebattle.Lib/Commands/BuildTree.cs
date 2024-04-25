using System.Collections;
using Hwdtech;

namespace Spacebattle.Lib;

public interface IVectorslike
{
    public List<List<int>> ToActualVectors();
}

public class BuildTree : ICommand
{
    private readonly IVectorslike _rawVectors;
    public BuildTree(IVectorslike vectorslike)
    {
        _rawVectors = vectorslike;
    }
    public void Execute()
    {
        Dictionary<bool, Action<Hashtable, int>> growTreeByCondition = new()
        {
            { false, new Action<Hashtable, int>((root, branch) => root.Add(branch, new Hashtable())) },
            { true, new Action<Hashtable, int>((root, branch) => {}) }
        };
        List<List<int>> vectors = _rawVectors.ToActualVectors();
        vectors.ForEach(vector =>
        {
            Hashtable? tree = IoC.Resolve<Hashtable>("Game.Collision.SetupTree");
            vector.ForEach(branch =>
            {
                bool checkKey = tree.ContainsKey(branch);
                growTreeByCondition[checkKey].Invoke(tree, branch);
                tree = (Hashtable?)tree[branch];
            });
        });
    }
}
