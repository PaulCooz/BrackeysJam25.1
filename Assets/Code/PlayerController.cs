using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace JamSpace
{
    public sealed class PlayerController : MonoBehaviour, FishZone.IChangeFishZone, GameManager.IGameStart
    {
        [SerializeField]
        private float speed = 0.25f;

        [SerializeField]
        private InputAction moveLeftInput;
        [SerializeField]
        private InputAction moveRightInput;
        [SerializeField]
        private InputAction fishingInput;

        [SerializeField]
        private StateSpriteAnimator spriteAnimator;

        [SerializeField]
        private SpriteRenderer boatSprite;
        [SerializeField]
        private SpriteRenderer humanSprite;
        [SerializeField]
        private SpriteRenderer fishSprite;
        [SerializeField]
        private SplineContainer spline;

        private UniTask? _currentTask;
        private bool     _isControlActive;
        private bool     _clickBeginOnPlayer;
        private bool     _insideFishZone;

        private Camera           _camera;
        private FishingMechanics _fishingMechanics;

        private void Awake()
        {
            _camera           = FindFirstObjectByType<Camera>();
            _fishingMechanics = FindFirstObjectByType<FishingMechanics>();

            fishSprite.transform.localScale = Vector3.zero;

            moveLeftInput.Enable();
            moveRightInput.Enable();
            fishingInput.Enable();
        }

        private void Update()
        {
            if (!_isControlActive)
                return;
            
            if (_currentTask.HasValue && !_currentTask.Value.Status.IsCompleted())
                return;


            var mouse                 = Mouse.current;
            var worldClickPos         = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
            var wasFishingAsClick     = false;
            var moveDirection         = (float?)null;
            var distFromClickToPlayer = worldClickPos.DistXY(transform.position);
            if (distFromClickToPlayer < 1.5f) // input on player
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    _clickBeginOnPlayer = true;
                    boatSprite.DOKill();
                    boatSprite.DOColor((0.7f * Color.white).WithA(1f), 0.3f);
                }
                if (mouse.leftButton.wasReleasedThisFrame)
                    wasFishingAsClick = _clickBeginOnPlayer;
            }
            if (!_clickBeginOnPlayer && mouse.leftButton.isPressed)
                moveDirection = Math.Clamp(worldClickPos.x - transform.position.x, -1, +1);
            if (mouse.leftButton.wasReleasedThisFrame)
            {
                if (_clickBeginOnPlayer)
                {
                    boatSprite.DOKill();
                    boatSprite.DOColor(Color.white, 0.3f);
                }
                _clickBeginOnPlayer = false;
            }

            if (moveLeftInput.IsPressed())
                moveDirection = -1;
            if (moveRightInput.IsPressed())
                moveDirection = +1;
            if (moveDirection.HasValue)
            {
                var bounds = GameManager.Instance.gameWorldRect;
                var pos    = transform.position;
                pos.x += speed * moveDirection.Value * Time.deltaTime;
                pos.x =  Mathf.Clamp(pos.x, bounds.xMin, bounds.xMax);

                transform.position = pos;
            }

            if (_insideFishZone && (wasFishingAsClick || fishingInput.WasPerformedThisFrame()))
            {
                spriteAnimator.Play("casting", false).Forget();
                spriteAnimator.defaultState = "waiting";
                var castingAnimDur = spriteAnimator.GetDuration("casting");
                var caughtAnimDur  = spriteAnimator.GetDuration("caught");
                _currentTask = _fishingMechanics.Run(
                    castingAnimDur, caughtAnimDur,
                    () => fishingInput.WasPerformedThisFrame() || mouse.leftButton.wasReleasedThisFrame
                ).ContinueWith(result =>
                {
                    const float show = 0.1f, fly = 0.8f, hide = 0.1f, step = 0.1f;

                    var seq = DOTween.Sequence();
                    seq.Append(fishSprite.transform.DOScale(1, caughtAnimDur * show));
                    for (var t = 0f; t <= 1f; t += step)
                        seq.Append(fishSprite.transform.DOMove(spline.EvaluatePosition(t), caughtAnimDur * (fly * step)));
                    seq.Append(fishSprite.transform.DOScale(0, caughtAnimDur * hide));

                    GameManager.Instance.Data.FishCount += result.PointsToAdd;
                    GameManager.Instance.Post<ICaughtFish>(l => l.PlayerCaughtFish(result));

                    spriteAnimator.defaultState = "idle";
                    return spriteAnimator.Play("caught", false);
                });
            }
        }

        void GameManager.IGameStart.GameStart() => _isControlActive = true;
        void FishZone.IChangeFishZone.PlayerEnter() => _insideFishZone = true;
        void FishZone.IChangeFishZone.PlayerExit()  => _insideFishZone = false;

        public interface ICaughtFish
        {
            void PlayerCaughtFish(FishingResult fishResult);
        }

        public void OnDamage()
        {
            OnDamageAsync().Forget();
        }

        private async UniTaskVoid OnDamageAsync()
        {
            _isControlActive = false;
            
            await DOTween.Sequence()
                .Join(boatSprite.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo))
                .Join(humanSprite.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo));
            
            GameManager.Instance.Finish(isWin: false);
        }
    }
}