using Hwdtech;

namespace Spacebattle.Lib;
public class SetFuelCapacityCommand: ICommand
{
    double _fuelCapacity;    
    public SetFuelCapacityCommand(double fuelCapacity)
    {
        _fuelCapacity = fuelCapacity;
    }
    public void Execute()
    {
        var gameObjects = IoC.Resolve<Dictionary<(int, int), IUObject>>("Game.Dictionary.PlayersAndShips");
        var _playersIds = IoC.Resolve<Iterator>("Game.Players.Ids");
        var _playersAmount = IoC.Resolve<int>("Game.Players.Amount");

        var gameObjectsPerPlayer = IoC.Resolve<int>("Game.GameObjectsPerPlayer");        

        Enumerable.Range(0, _playersAmount).ToList().ForEach(playerId => {

            Enumerable.Range(0, gameObjectsPerPlayer).ToList().ForEach(shipId => {
                
                var position = new Vector(new int[] {shipId, (int) _playersIds.Current});
                IoC.Resolve<ICommand>("Game.Commands.SetProperty", ((int)_playersIds.Current, shipId), "Fuel", _fuelCapacity).Execute();
            });
            
            _playersIds.MoveNext();
        });
    }
}
