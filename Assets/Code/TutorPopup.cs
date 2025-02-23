using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class TutorPopup : MonoBehaviour, GameManager.IGameStart
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        private UniTaskCompletionSource _completion;

        private Tween _anim;

        private void Awake()
        {
            canvasGroup.alpha          = 0;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
        }

        public UniTask GameStartAsync()
        {
            if (GameManager.Instance.Data.ShownFirstTutor)
                return UniTask.CompletedTask;

            _completion = new UniTaskCompletionSource();
            Show();
            return _completion.Task;
        }

        private void Show()
        {
            _anim = canvasGroup.DOFade(1f, 0.3f).OnComplete(() =>
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
            _anim = canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
            {
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;

                GameManager.Instance.Data.ShownFirstTutor = true;
                _completion?.TrySetResult();

                _anim = null;
            });
        }
    }
}