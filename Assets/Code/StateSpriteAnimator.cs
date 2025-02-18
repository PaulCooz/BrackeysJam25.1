using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamSpace
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class StateSpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private List<AnimSet> anims;
        [SerializeField]
        private float delay = 0.1f;
        [SerializeField]
        public string defaultState;

        private Dictionary<string, AnimSet> _stateAnim;

        private SpriteRenderer _renderer;

        private int       _currentSprite;
        private TimeUntil _nextSprite;

        private (Sprite[] sprites, bool looped, UniTaskCompletionSource complete)? _curr, _next;

        private void Awake()
        {
            _renderer  = GetComponent<SpriteRenderer>();
            _stateAnim = anims.ToDictionary(i => i.state, i => i);
        }

        public UniTask Play(string state, bool looped = true)
        {
            _curr?.complete.TrySetResult();
            _curr          = null;
            _currentSprite = 0;

            _next = (_stateAnim[state].sprites, looped, new UniTaskCompletionSource());
            return _next.Value.complete.Task;
        }

        private void Update()
        {
            if (!_curr.HasValue)
            {
                _curr = _next ?? (_stateAnim[defaultState].sprites, true, new UniTaskCompletionSource());
                _next = null;
            }

            if (_nextSprite)
            {
                var curr = _curr.Value;
                _currentSprite = (_currentSprite + 1) % (curr.looped ? curr.sprites.Length : int.MaxValue);
                if (_currentSprite < curr.sprites.Length)
                {
                    _renderer.sprite = curr.sprites[_currentSprite];
                    _nextSprite      = delay;
                }
                else
                {
                    curr.complete.TrySetResult();
                    _curr          = null;
                    _currentSprite = 0;
                }
            }
        }

        public float GetDuration(string state) => _stateAnim[state].sprites.Length * delay;

        [Serializable]
        private class AnimSet
        {
            public string   state;
            public Sprite[] sprites;
        }
    }
}