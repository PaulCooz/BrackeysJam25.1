using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public class EnemiesSpawnController : MonoBehaviour, GameManager.IGameStart
    {
        private const float EnemyLayer = -3f;
        
        [SerializeField] private List<GameObject> enemiesPrefabs = new();
        [SerializeField] private Transform PlayerTarget;
        [SerializeField] private float MinSpawnInterval;
        [SerializeField] private float MaxSpawnInterval;

        public void GameStart()
        {
            SpawnEnemiesAsync().Forget();
        }

        private async UniTaskVoid SpawnEnemiesAsync()
        {
            while (true)
            {
                await UniTask.WaitForSeconds(Random.Range(MinSpawnInterval, MaxSpawnInterval));
                
                Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)]).GetComponent<IEnemy>().Attack(worldPositionToAttack: new Vector3(PlayerTarget.position.x, PlayerTarget.position.y, EnemyLayer));
            }
        }
    }
}