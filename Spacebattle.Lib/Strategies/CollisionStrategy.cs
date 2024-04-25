using Hwdtech;

namespace Spacebattle.Lib;
public class CollisionStrategy
{
    private static readonly object[] defaultTreeEntry = new object[]
    {
            new Dictionary<int, object[]>(),
            (Action<object[]>) ((object[] args) => {return;})
    };

    public static void ContinueSearch(object[] args)
    {
        var tree = (Dictionary<int, object[]>)args[0];
        var key = (IEnumerator<int>)args[1];

        var entry = tree.GetValueOrDefault(key.Current, defaultTreeEntry);
        args[0] = entry[0];

        var command = (Action<object[]>)entry[1];
        key.MoveNext();
        command(args);
    }

    public static void Leaf(object[] args) =>
        IoC.Resolve<ICommand>(
            "Objects.Collision.Resolve",
            args[2],
            args[3]
        ).Execute();

    public static object DetectCollision(object[] args)
    {
        var obj1 = args[0];
        var obj2 = args[1];
        var keys = IoC.Resolve<IEnumerator<int>>("Objects.RelativeVector.Get", obj1, obj2);
        object tree = IoC.Resolve<Dictionary<int, object[]>>("Objects.Collision.Tree.Get");
        keys.MoveNext();

        return new ActionCommand(CollisionStrategy.ContinueSearch, new object[] { tree, keys, obj1, obj2 });
    }

    public static object ResolveCollision(object[] _)
    {
        return new ActionCommand((object[] _) => { throw new Exception(); }, new object[] { });
    }

    public static object GetRelativeVector(object[] args)
    {
        var obj1 = (IUObject)args[0];
        var obj2 = (IUObject)args[1];

        var velocityVector =
            (Vector)obj2.GetProperty("Velocity") -
            (Vector)obj1.GetProperty("Velocity");

        var positionVector =
            (Vector)obj2.GetProperty("Position") -
            (Vector)obj1.GetProperty("Position");

        var relativeVector = Vector.Concat(positionVector, velocityVector);

        return Vector.GetEnumerator(relativeVector);
    }
}
