using System;
using System.Collections.Generic;
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
        private CanvasGroup canvasGroup;
        [SerializeField]
        private ImageAnimator animator;

        [SerializeField]
        private ViewData winData, loseData;

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
            animator.sprites = viewData.sprites.Rand().sprites;

            Show();
        }

        public void Show()
        {
            canvasGroup.DOFade(1f, 0.3f).OnComplete(() =>
            {
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;
            });
        }

        public void Hide()
        {
            canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
            {
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;
            });
        }

        [Serializable]
        private struct ViewData
        {
            public string       title;
            public string       body;
            public SpriteList[] sprites;
        }

        [Serializable]
        private struct SpriteList
        {
            public Sprite[] sprites;
        }
    }
}