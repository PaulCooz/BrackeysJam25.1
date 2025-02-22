using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public sealed class FishZoneSystem : MonoBehaviour, GameManager.IGameStart, FishZone.IChangeFishZone
    {
        [SerializeField]
        private FishZone zonePrefab;
        [SerializeField]
        private Transform worldTransform;
        [SerializeField]
        private GameObject fishStatusIcon;

        public void GameStart()
        {
            fishStatusIcon.SetActive(false);
            StartSpawningAsync().Forget();
        }

        private async UniTask StartSpawningAsync()
        {
            var manager  = GameManager.Instance;
            var settings = manager.LevelSettings.fishZone;
            while (this.IsAlive())
            {
                var spawnDelay = UnityEngine.Random.Range(settings.minMaxSpawnSec.x, settings.minMaxSpawnSec.y);
                await UniTask.WaitForSeconds(spawnDelay);

                var zone = Instantiate(zonePrefab, worldTransform);

                zone.Setup(UnityEngine.Random.Range(settings.minMaxDurationSec.x, settings.minMaxDurationSec.y));
                zone.transform.position = new Vector3(
                    UnityEngine.Random.Range(manager.gameWorldRect.xMin, manager.gameWorldRect.xMax),
                    Mathf.Lerp(manager.gameWorldRect.yMin, manager.gameWorldRect.yMax, 0.5f)
                );
            }
        }

        public void PlayerEnter() => fishStatusIcon.SetActive(true);
        public void PlayerExit()  => fishStatusIcon.SetActive(false);

        [Serializable]
        public struct Settings
        {
            public Vector2 minMaxSpawnSec;
            public Vector2 minMaxDurationSec;
        }
    }
}