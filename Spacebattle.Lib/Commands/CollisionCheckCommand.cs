using Hwdtech;

namespace Spacebattle.Lib;
public class CollisionCheckCommand : ICommand
{
    private readonly IUObject _firstObject, _secondObject;
    public CollisionCheckCommand(IUObject obj1, IUObject obj2)
    {
        _firstObject = obj1;
        _secondObject = obj2;
    }

    public void Execute() => IoC.Resolve<ICommand>("Objects.Collision.Check", _firstObject, _secondObject).Execute();
}
