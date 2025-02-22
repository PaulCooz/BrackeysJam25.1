using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class LevelResultSystem : MonoBehaviour, GameManager.ILevelFinish
    {
        [SerializeField]
        private TMP_Text titleTMP;
        [SerializeField]
        private TMP_Text bodyTMP;
        [SerializeField]
        private TMP_Text buttonTMP;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private ImageAnimator animator;

        [SerializeField]
        private ViewData winData, loseData;

        private Action _clickAction;
        private Tween  _anim;

        private void Awake()
        {
            canvasGroup.alpha          = 0;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void LevelFinish(GameManager.LevelResult result)
        {
            var viewData = result.IsWin ? winData : loseData;
            titleTMP.text    = viewData.title;
            bodyTMP.text     = viewData.body;
            buttonTMP.text   = viewData.button;
            animator.sprites = viewData.sprites.Rand().sprites;

            _clickAction = result.IsWin
                ? GameManager.Instance.NextLevel
                : () => GameManager.Instance.ReplayLevel().Forget();

            Show();
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
            _clickAction();
        }

        private void Hide()
        {
            _anim = canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
            {
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;

                _anim = null;
            });
        }

        [Serializable]
        private struct ViewData
        {
            public string       title;
            public string       body;
            public string       button;
            public SpriteList[] sprites;
        }

        [Serializable]
        private struct SpriteList
        {
            public Sprite[] sprites;
        }
    }
}