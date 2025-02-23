using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class TutorFishingPopup : MonoBehaviour, FishZone.IChangeFishZone
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        private Tween _anim;

        private void Awake()
        {
            canvasGroup.alpha          = 0;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void PlayerEnter()
        {
            if (GameManager.Instance.Data.ShownFishingTutor)
                return;

            Time.timeScale = 0;
            Show();
        }

        public void PlayerExit() { }

        private void Show()
        {
            _anim = canvasGroup.DOFade(1f, 0.3f).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
            {
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;

                _anim = null;
            });
        }

        public void OnClick()
        {
            if (_anim is not null)
                return;

            Hide();
        }

        private void Hide()
        {
            _anim = canvasGroup.DOFade(0f, 0.3f).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
            {
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;

                GameManager.Instance.Data.ShownFishingTutor = true;

                Time.timeScale = 1;

                _anim = null;
            });
        }
    }
}