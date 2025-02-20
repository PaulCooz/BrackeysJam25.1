using UnityEngine;

namespace JamSpace
{
    public sealed class LevelCompleteSystem : MonoBehaviour, GameManager.IGameStart, PlayerController.ICaughtFish
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
                GameManager.Instance.Finish(true);
            }
        }
    }
}