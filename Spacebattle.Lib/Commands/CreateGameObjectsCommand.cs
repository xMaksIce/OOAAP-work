using Hwdtech;

namespace Spacebattle.Lib;
public class CreateGameObjectsCommand : ICommand
{
    int _gameObjectsPerPlayer;
    int _playersAmount;
    public CreateGameObjectsCommand(int playersAmount, int gameObjectsPerPlayer)
    {
        _gameObjectsPerPlayer = gameObjectsPerPlayer;
        _playersAmount = playersAmount;
    }
    public void Execute()
    {
        var gameObjects = IoC.Resolve<Dictionary<(int, int), IUObject>>("Game.Dictionary.PlayersAndShips");

        Enumerable.Range(0, _playersAmount).ToList().ForEach(playerId =>
        {
            Enumerable.Range(0, _gameObjectsPerPlayer).ToList().ForEach(gameObjectId =>
                {
                    gameObjects.Add((playerId, gameObjectId), IoC.Resolve<IUObject>("Game.GameObject.Create"));
                });
        });
    }
}
