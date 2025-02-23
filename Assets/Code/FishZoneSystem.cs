using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public sealed class FishZoneSystem : MonoBehaviour, GameManager.ILevelStart, GameManager.ILevelFinish, FishZone.IChangeFishZone,
        GameManager.ILevelReplay
    {
        [SerializeField]
        private FishZone zonePrefab;
        [SerializeField]
        private Transform worldTransform;
        [SerializeField]
        private GameObject fishStatusIcon;

        private CancellationTokenSource _cancel;

        public void LevelStart()
        {
            fishStatusIcon.SetActive(false);
            _cancel = new CancellationTokenSource();
            StartSpawningAsync().Forget();
        }

        private async UniTask StartSpawningAsync()
        {
            var manager  = GameManager.Instance;
            var settings = manager.LevelSettings.fishZone;
            while (!_cancel.IsCancellationRequested && this.IsAlive())
            {
                var spawnDelay = UnityEngine.Random.Range(settings.minMaxSpawnSec.x, settings.minMaxSpawnSec.y);
                await UniTask.WaitForSeconds(spawnDelay, cancellationToken: _cancel.Token);

                var zone = Instantiate(zonePrefab, worldTransform);

                zone.Setup(UnityEngine.Random.Range(settings.minMaxDurationSec.x, settings.minMaxDurationSec.y));
                zone.transform.position = new Vector3(
                    UnityEngine.Random.Range(manager.gameWorldRect.xMin, manager.gameWorldRect.xMax),
                    Mathf.Lerp(manager.gameWorldRect.yMin, manager.gameWorldRect.yMax, 0.5f)
                );
            }
            _cancel = null;
        }

        public void PlayerEnter() => fishStatusIcon.SetActive(true);
        public void PlayerExit()  => fishStatusIcon.SetActive(false);

        public void LevelFinish(GameManager.LevelResult result) => CancelSpawning();
        public  void LevelReplay() => CancelSpawning();
        private void CancelSpawning() => _cancel?.Cancel();

        [Serializable]
        public struct Settings
        {
            public Vector2 minMaxSpawnSec;
            public Vector2 minMaxDurationSec;
        }
    }
}