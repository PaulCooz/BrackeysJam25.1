using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class SpeedIndicator : MonoBehaviour, GameManager.ILevelStart, PlayerController.IChangeSpeed
    {
        int IOrdered.Order => int.MaxValue;

        [SerializeField]
        private Image indicator;
        [SerializeField]
        private Sprite fastSprite;
        [SerializeField]
        private Sprite slowSprite;

        private PlayerController _player;

        public void LevelStart()
        {
            _player = FindFirstObjectByType<PlayerController>();
            UpdateIndicator();
        }

        public void PlayerChangeSpeed(int _) => UpdateIndicator();

        private void UpdateIndicator()
        {
            if (_player.CurrentSpeed > _player.speed)
            {
                indicator.sprite  = fastSprite;
                indicator.enabled = true;
            }
            else if (_player.CurrentSpeed < _player.speed)
            {
                indicator.sprite  = slowSprite;
                indicator.enabled = true;
            }
            else
            {
                indicator.enabled = false;
            }
        }
    }
}