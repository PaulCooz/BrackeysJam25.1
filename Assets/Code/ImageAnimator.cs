using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    [RequireComponent(typeof(Image))]
    public sealed class ImageAnimator : BaseSpriteAnimator
    {
        private Image _image;

        public override void OnAwake() => _image = GetComponent<Image>();

        public override void OnSetSprite(Sprite sprite) => _image.sprite = sprite;
    }
}