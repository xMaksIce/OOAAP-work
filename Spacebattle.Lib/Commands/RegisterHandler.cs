using System.Collections;
using Hwdtech;

namespace Spacebattle.Lib;

public class RegisterExceptionHandler : ICommand
{
    private readonly IEnumerable<object> cmdOrExcTrail;
    private readonly ICommand handler;
    public RegisterExceptionHandler(IEnumerable<object> cmdOrExcTrail, ICommand handler)
    {
        this.cmdOrExcTrail = cmdOrExcTrail;
        this.handler = handler;
    }

    public void Execute()
    {
        Hashtable? tree = IoC.Resolve<Hashtable>("Game.Handle.GetTree");

        Dictionary<bool, Action<Type>> branching = new(){
            {true, new Action<Type>((nodeType) => {
                if (!tree.ContainsKey(nodeType))
                    tree.Add(nodeType, new Hashtable());
                tree = (Hashtable?) tree[nodeType];})},
            {false, new Action<Type>((nodeType) => {
                if (!tree.ContainsKey(nodeType))
                    tree.Add(nodeType, handler);})}};

        cmdOrExcTrail.ToList().ForEach((node) =>
        {
            Type typeOfNode = node.GetType();
            bool isNodeACommand = typeOfNode.IsAssignableTo(typeof(ICommand));
            branching[isNodeACommand].Invoke(typeOfNode);
        });
    }
}
