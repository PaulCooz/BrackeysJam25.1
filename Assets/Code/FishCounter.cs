using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class FishCounter : MonoBehaviour, GameManager.ILevelStart, PlayerController.ICaughtFish
    {
        [SerializeField]
        private TMP_Text tmp;
        [SerializeField]
        private string format = "{0} fish from {1}";

        private void Start()
        {
            UniTask.NextFrame().ContinueWith(LevelStart).Forget();
        }

        public void LevelStart()
        {
            tmp.text = string.Format(format, GameManager.Instance.Data.FishCount, GameManager.Instance.Data.FishToCollect);
        }

        public void PlayerCaughtFish()
        {
            tmp.text = string.Format(format, GameManager.Instance.Data.FishCount, GameManager.Instance.Data.FishToCollect);
        }
    }
}