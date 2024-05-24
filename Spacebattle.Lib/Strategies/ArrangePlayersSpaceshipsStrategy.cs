using Hwdtech.Ioc;
using Hwdtech;
// using 

namespace Spacebattle.Lib;
public class ArrangeSpaceshipsStrategy: IStrategy
{
    object Run(object[] args)
    {
        var axisY = IoC.Resolve<double>("Game.GameObjectsArrangement.AxisY");
        var pos = new int[] {1, 2};
        // 3 ships to one player
        new Vector(pos);
    }
}