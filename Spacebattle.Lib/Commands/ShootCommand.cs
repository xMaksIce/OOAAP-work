using Hwdtech;
namespace Spacebattle.Lib;
public interface IShootable
{
    string TorpedaType { get; set; }
}
public class ShootCommand : ICommand
{
    readonly private IShootable _shootable;
    public ShootCommand(IShootable shootable) { _shootable = shootable; }
    public void Execute()
    {
        var torpeda = IoC.Resolve<object>("Game.Shoot.Torpeda", _shootable);
        IoC.Resolve<ICommand>("Game.Shoot").Execute();
    }
}
