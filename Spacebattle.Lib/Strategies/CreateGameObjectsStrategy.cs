using Hwdtech.Ioc;
using Moq;
using Hwdtech;

namespace Spacebattle.Lib;
public class CreateGameObjectsStrategy: IStrategy
{
    public object Run(object[] args)
    {
        var gameObject = Enumerable.Range(1, (int) args[0]).Select(gObj => 
                {
                    var gObject = new Mock<IMoveCommandStartable>();
                    gObject.SetupGet(obj => obj.Property).Returns(new Dictionary<string, object>());
                    gObject.SetupGet(obj => obj.Target).Returns(new Mock<IUObject>().Object);
                    var id = Guid.NewGuid();
                    IoC.Resolve<Hwdtech.ICommand>("IoC.Register", $"Game.GameObject{id}",
                        (object[] args) => gObject.Object
                    ).Execute();
                    return gObject;
                });
         
        return gameObject;
    }
}
