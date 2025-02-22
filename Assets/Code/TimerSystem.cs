using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class TimerSystem : MonoBehaviour, GameManager.IGameStart
    {
        [SerializeField]
        private TMP_Text tmp;

        public void GameStart()
        {
            StartTimer().Forget();
        }

        private async UniTask StartTimer()
        {
            var gameManager = GameManager.Instance;
            var data        = gameManager.Data;
            while (this.IsAlive())
            {
                var span = data.TimerToGameOver;
                tmp.text = $"{span.Minutes}:{span.Seconds}";

                await UniTask.NextFrame();

                span =  data.TimerToGameOver;
                span -= TimeSpan.FromSeconds(Time.deltaTime);

                data.TimerToGameOver = span;
                if (data.TimerToGameOver == TimeSpan.Zero)
                {
                    gameManager.Finish(false);
                }
            }
        }
    }
}