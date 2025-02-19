using UnityEngine;

namespace JamSpace
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimator : BaseSpriteAnimator
    {
        private SpriteRenderer _renderer;

        public override void OnAwake() => _renderer = GetComponent<SpriteRenderer>();

        public override void OnSetSprite(Sprite sprite) => _renderer.sprite = sprite;
    }
}