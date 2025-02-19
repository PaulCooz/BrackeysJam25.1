using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class FishCounter : MonoBehaviour, GameManager.IGameStart, PlayerController.ICaughtFish
    {
        [SerializeField]
        private TMP_Text tmp;
        [SerializeField]
        private string format = "{0} fish from {1}";

        public void GameStart()
        {
            tmp.text = string.Format(format, GameManager.Instance.Data.FishCount, GameManager.Instance.Data.FishToCollect);
        }

        public void PlayerCaughtFish(FishingResult fishResult)
        {
            tmp.text = string.Format(format, GameManager.Instance.Data.FishCount, GameManager.Instance.Data.FishToCollect);
        }
    }
}