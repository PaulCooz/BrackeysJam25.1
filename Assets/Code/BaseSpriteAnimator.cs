using UnityEngine;

namespace JamSpace
{
    public abstract class BaseSpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        public Sprite[] sprites;
        [SerializeField]
        private float delay = 0.1f;
        [SerializeField]
        private int startFrom = 0;

        private int       _currentSprite;
        private TimeUntil _nextSprite;

        public abstract void OnAwake();
        public abstract void OnSetSprite(Sprite sprite);

        private void Awake()
        {
            OnAwake();
            _currentSprite = startFrom;
            _nextSprite    = delay;
        }

        private void Update()
        {
            if (_nextSprite)
            {
                _currentSprite = (_currentSprite + 1) % sprites.Length;
                OnSetSprite(sprites[_currentSprite]);
                _nextSprite = delay;
            }
        }
    }
}