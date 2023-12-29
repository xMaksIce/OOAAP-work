using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Spacebattle.Lib;

namespace Spacebattle.Tests;
public class DetectCollisionCommandTest
{
    private static void BuildAndRegisterTree()
    {
        var collisionTree =
        new Dictionary<int, object[]>()
        {{
            10,
            new object[]
            {
                new Dictionary<int, object[]>()
                {{
                    -2,
                    new object[]
                    {
                        new Dictionary<int, object[]>()
                        {{
                            5,
                            new object[]
                            {
                                new Dictionary<int, object[]>()
                                {{
                                    -8,
                                    new object[]
                                    {
                                        new Dictionary<int, object[]>(),
                                        new Action<object[]>(CollisionStrategy.Leaf)
                                    }
                                }},
                                new Action<object[]>(CollisionStrategy.ContinueSearch)
                            }
                        }},
                        new Action<object[]>(CollisionStrategy.ContinueSearch)
                    }
                }},
                new Action<object[]>(CollisionStrategy.ContinueSearch)
            }
        }};

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Objects.Collision.Tree.Get",
            (object[] args) => collisionTree
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Objects.Collision.Check",
            (object[] args) => CollisionStrategy.DetectCollision(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Objects.RelativeVector.Get",
            (object[] args) => CollisionStrategy.GetRelativeVector(args)
        ).Execute();

    }

    private static void InitialState()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        BuildAndRegisterTree();

    }

    [Fact]
    public void ThereIsCollisionTest()
    {
        InitialState();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Objects.Collision.Resolve",
            (object[] args) => CollisionStrategy.ResolveCollision(args)
        ).Execute();

        var obj1 = new Mock<IUObject>();
        var obj2 = new Mock<IUObject>(); // 10 -2 5 -8

        obj1.Setup(x => x.GetProperty("Position")).Returns(new Vector(new int[] { 20, 7 }));
        obj1.Setup(x => x.GetProperty("Velocity")).Returns(new Vector(new int[] { 5, 18 }));

        obj2.Setup(x => x.GetProperty("Position")).Returns(new Vector(new int[] { 30, 5 }));
        obj2.Setup(x => x.GetProperty("Velocity")).Returns(new Vector(new int[] { 10, 10 }));

        Spacebattle.Lib.ICommand command = new CollisionCheckCommand(obj1.Object, obj2.Object);

        Assert.Throws<Exception>(() => command.Execute());
    }

    [Fact]
    public void ThereIsNoCollisionTest()
    {
        InitialState();

        var collisionCommand = new Mock<Spacebattle.Lib.ICommand>();
        collisionCommand.Setup(x => x.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Objects.Collision.Resolve",
            (object[] args) => collisionCommand.Object
        ).Execute();

        var obj1 = new Mock<IUObject>();
        var obj2 = new Mock<IUObject>();

        obj1.Setup(x => x.GetProperty("Position")).Returns(new Vector(new int[] { 2, 2 }));
        obj1.Setup(x => x.GetProperty("Velocity")).Returns(new Vector(new int[] { 7, 6 }));

        obj2.Setup(x => x.GetProperty("Position")).Returns(new Vector(new int[] { 3, 5 }));
        obj2.Setup(x => x.GetProperty("Velocity")).Returns(new Vector(new int[] { 10, 10 }));

        Spacebattle.Lib.ICommand command = new CollisionCheckCommand(obj1.Object, obj2.Object);
        command.Execute();

        collisionCommand.Verify(x => x.Execute(), Times.Never);
    }
}
