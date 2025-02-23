using UnityEngine;

namespace JamSpace
{
    public sealed class LevelCompleteSystem : MonoBehaviour, GameManager.ILevelStart, PlayerController.ICaughtFish
    {
        private GameData _data;

        public void LevelStart()
        {
            _data = GameManager.Instance.Data;
        }

        public void PlayerCaughtFish()
        {
            if (_data.FishCount == _data.FishToCollect)
            {
                GameManager.Instance.Finish(true);
            }
        }
    }
}