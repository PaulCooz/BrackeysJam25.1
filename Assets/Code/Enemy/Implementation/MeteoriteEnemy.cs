using DG.Tweening;
using UnityEngine;

namespace JamSpace.Implementation
{
    public class MeteoriteEnemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private Transform explosionVfx;
        
        public void Attack(Vector3 worldPositionToAttack)
        {
            transform.position = worldPositionToAttack;
            rigidBody.position = new Vector2(Random.Range(-10f,10f), 30);
            
            DOTween.Sequence()
                .Append(rigidBody.DOMove(endValue: worldPositionToAttack, duration: Random.Range(5f, 10f)))
                .AppendCallback(() => explosionVfx.gameObject.SetActive(true))
                .AppendInterval(1.5f)
                .AppendCallback(() => rigidBody.gameObject.SetActive(false))
                .AppendInterval(3f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}