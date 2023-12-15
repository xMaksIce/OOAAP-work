using Hwdtech;

namespace Spacebattle.Lib;

public class CollisionCheckCommand : ICommand
{
    private readonly IUObject _first, _second;

    public CollisionCheckCommand(IUObject _first, IUObject _second)
    {
        this._first = _first;
        this._second = _second;
    }

    public void Execute()
    {

        bool result = IoC.Resolve<bool>("Events.Collision.Determinant", _first, _second);

        if (result)
        {
            throw new Exception(string.Format("There was a conflict between {0} and {1}", _first.ToString(), _second.ToString()));
        }
    }
}