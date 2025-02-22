using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class TimerSystem : MonoBehaviour, GameManager.IGameStart, GameManager.ILevelFinish, GameManager.ILevelReplay
    {
        [SerializeField]
        private TMP_Text tmp;

        private CancellationTokenSource _cancel;

        public void GameStart()
        {
            _cancel = new CancellationTokenSource();
            StartTimer().Forget();
        }

        private async UniTask StartTimer()
        {
            var gameManager = GameManager.Instance;
            var data        = gameManager.Data;
            while (!_cancel.IsCancellationRequested && this.IsAlive())
            {
                var span = data.TimerToGameOver;
                tmp.text = $"{span.Minutes}:{span.Seconds}";

                await UniTask.NextFrame(cancellationToken: _cancel.Token);

                span =  data.TimerToGameOver;
                span -= TimeSpan.FromSeconds(Time.deltaTime);

                data.TimerToGameOver = span;
                if (data.TimerToGameOver == TimeSpan.Zero)
                {
                    gameManager.Finish(false);
                }
            }
            _cancel = null;
        }

        public void LevelFinish(GameManager.LevelResult _) => CancelTimer();
        public void LevelReplay() => CancelTimer();
        private void CancelTimer() => _cancel?.Cancel();
    }
}