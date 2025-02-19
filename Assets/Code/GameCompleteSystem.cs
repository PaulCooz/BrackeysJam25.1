using UnityEngine;

namespace JamSpace
{
    public sealed class GameCompleteSystem : MonoBehaviour, GameManager.IGameStart, PlayerController.ICaughtFish
    {
        private GameData _data;

        public void GameStart()
        {
            _data = GameManager.Instance.Data;
        }

        public void PlayerCaughtFish(FishingResult fishResult)
        {
            if (_data.FishCount == _data.FishToCollect)
            {
                var levelRes = new GameManager.LevelResult(true, _data.Level);
                GameManager.Instance.Post<GameManager.ILevelFinish>(l => l.LevelFinish(levelRes));
            }
        }
    }
}