using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    public sealed class FishZone : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles;

        public void Setup(float lifetime)
        {
            particles.Play();

            UniTask
                .WaitForSeconds(lifetime, cancellationToken: this.GetCancellationTokenOnDestroy())
                .ContinueWith(() =>
                {
                    if (this.IsAlive())
                    {
#if UNITY_EDITOR
                        if (!UnityEditor.EditorApplication.isPlaying) // при остановке игры в редакторе не удаляет объект
                            DestroyImmediate(gameObject);
                        else
#endif
                            Destroy(gameObject);
                    }
                })
                .Forget();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.parent.CompareTag("Player"))
            {
                GameManager.Instance.Post<IChangeFishZone>(l => l.PlayerEnter());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.parent.CompareTag("Player"))
            {
                GameManager.Instance.Post<IChangeFishZone>(l => l.PlayerExit());
            }
        }

        public interface IChangeFishZone
        {
            public void PlayerEnter();
            public void PlayerExit();
        }
    }
}