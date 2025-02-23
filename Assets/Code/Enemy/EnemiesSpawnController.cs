using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public class EnemiesSpawnController : MonoBehaviour, GameManager.ILevelStart, GameManager.ILevelFinish,
        GameManager.ILevelReplay
    {
        private const float EnemyLayer = -3f;

        [SerializeField]
        private List<GameObject> enemiesPrefabs = new ();
        [SerializeField]
        private Transform PlayerTarget;

        private CancellationTokenSource _cancel;

        public void LevelStart()
        {
            _cancel = new CancellationTokenSource();
            SpawnEnemiesAsync().Forget();
        }

        private async UniTaskVoid SpawnEnemiesAsync()
        {
            while (!_cancel.IsCancellationRequested && this.IsAlive())
            {
                var settings      = GameManager.Instance.LevelSettings;
                var spawnInterval = Random.Range(settings.minMaxSpawnMeteorInterval.x, settings.minMaxSpawnMeteorInterval.y);
                await UniTask.WaitForSeconds(spawnInterval, cancellationToken: _cancel.Token);

                var speed = Random.Range(settings.minMaxSpawnMeteorSpeed.x, settings.minMaxSpawnMeteorSpeed.y);
                Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], new Vector3(0, 0, EnemyLayer), Quaternion.identity)
                    .GetComponent<IEnemy>()
                    .Attack(worldPositionToAttack: new Vector3(PlayerTarget.position.x, PlayerTarget.position.y, EnemyLayer), speed);
            }
            _cancel = null;
        }

        public void LevelFinish(GameManager.LevelResult _) => CancelSpawning();
        public void LevelReplay() => CancelSpawning();
        private void CancelSpawning() => _cancel?.Cancel();
    }
}