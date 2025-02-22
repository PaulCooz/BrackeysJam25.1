using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public class EnemiesSpawnController : MonoBehaviour, GameManager.IGameStart, GameManager.ILevelFinish,
        GameManager.ILevelReplay
    {
        private const float EnemyLayer = -3f;

        [SerializeField]
        private List<GameObject> enemiesPrefabs = new ();
        [SerializeField]
        private Transform PlayerTarget;
        [SerializeField]
        private float MinSpawnInterval;
        [SerializeField]
        private float MaxSpawnInterval;

        private CancellationTokenSource _cancel;

        public void GameStart()
        {
            _cancel = new CancellationTokenSource();
            SpawnEnemiesAsync().Forget();
        }

        private async UniTaskVoid SpawnEnemiesAsync()
        {
            while (!_cancel.IsCancellationRequested && this.IsAlive())
            {
                var spawnInterval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
                await UniTask.WaitForSeconds(spawnInterval, cancellationToken: _cancel.Token);
                
                Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], new Vector3(0, 0, EnemyLayer), Quaternion.identity)
                    .GetComponent<IEnemy>()
                    .Attack(worldPositionToAttack: new Vector3(PlayerTarget.position.x, PlayerTarget.position.y, EnemyLayer));
            }
            _cancel = null;
        }

        public void LevelFinish(GameManager.LevelResult _) => CancelSpawning();
        public void LevelReplay() => CancelSpawning();
        private void CancelSpawning() => _cancel?.Cancel();
    }
}