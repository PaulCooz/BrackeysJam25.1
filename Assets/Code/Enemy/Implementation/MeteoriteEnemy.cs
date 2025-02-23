using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamSpace.Implementation
{
    public class MeteoriteEnemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private ParticleSystem defaultVfx;
        [SerializeField] private Transform explosionVfx;
        [SerializeField] private SpriteRenderer shadow;
        
        public void Attack(Vector3 worldPositionToAttack, float speed)
        {
            transform.position = worldPositionToAttack;
            
            var meteoriteStartPos = new Vector3(Random.Range(-10f,10f), GameManager.Instance.gameWorldRect.height + 4, worldPositionToAttack.z);
            rigidBody.transform.position = meteoriteStartPos;
            rigidBody.position = meteoriteStartPos;
            
            DOTween.Sequence()
                .Append(rigidBody
                    .DOMove(endValue: worldPositionToAttack, duration: speed)
                    .SetEase(Ease.Linear))
                .AppendCallback(() => explosionVfx.gameObject.SetActive(true))
                .AppendInterval(0.1f)
                .AppendCallback(() =>
                {
                    rigidBody.gameObject.SetActive(false);
                    shadow.gameObject.SetActive(false);
                })
                .AppendInterval(1f)
                .OnKill(() =>
                {
                    DOTween.Kill(gameObject);
                    Destroy(gameObject);
                });

            shadow.transform.DOShakeScale(duration: 10f, strength: 0.1f, vibrato: 5).SetId(gameObject);
            shadow.DOFade(1f, duration: 2f).From(0f).SetEase(Ease.Linear).SetId(gameObject);

            EnableDefaultVfxAfterPhysicsUpdateAsync().Forget();
        }

        private async UniTaskVoid EnableDefaultVfxAfterPhysicsUpdateAsync()
        {
            await UniTask.NextFrame();
            await UniTask.NextFrame();
            
            defaultVfx.gameObject.SetActive(true);
            defaultVfx.Play();
        }

        private void Update()
        {
            var shadowPos = shadow.transform.position;
            shadowPos.x = rigidBody.transform.position.x;
            shadow.transform.position = shadowPos;
        }
    }
}