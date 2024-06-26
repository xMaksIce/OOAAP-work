using Hwdtech;

namespace Spacebattle.Lib;
public class PlacePlayersShipsCommand : ICommand
{
    public void Execute()
    {
        var _playersIds = IoC.Resolve<Iterator>("Game.Players.Ids");
        var _playersAmount = IoC.Resolve<int>("Game.Players.Amount");
        var gameObjectsPerPlayer = IoC.Resolve<int>("Game.GameObjectsPerPlayer");
        var gameObjects = IoC.Resolve<Dictionary<(int, int), IUObject>>("Game.Dictionary.PlayersAndShips");

        Enumerable.Range(0, _playersAmount).ToList().ForEach(playerId =>
        {

            Enumerable.Range(0, gameObjectsPerPlayer).ToList().ForEach(shipId =>
            {
                var position = new Vector(new int[] { shipId, (int)_playersIds.Current });
                var playerAndShipId = ((int)_playersIds.Current, shipId);
                IoC.Resolve<ICommand>("Game.Commands.SetProperty", playerAndShipId, "Position", position).Execute();
            });

            _playersIds.MoveNext();
        }
        );

        _playersIds.Reset();
    }
}
