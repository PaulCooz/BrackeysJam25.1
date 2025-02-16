using UnityEngine;

namespace JamSpace
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] sprites;
        [SerializeField]
        private float delay = 0.1f;
        [SerializeField]
        private int startFrom = 0;

        private int       _currentSprite;
        private TimeUntil _nextSprite;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer      = GetComponent<SpriteRenderer>();
            _currentSprite = startFrom;
            _nextSprite    = delay;
        }

        private void Update()
        {
            if (_nextSprite)
            {
                _currentSprite   = (_currentSprite + 1) % sprites.Length;
                _renderer.sprite = sprites[_currentSprite];
                _nextSprite      = delay;
            }
        }
    }
}