using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace JamSpace
{
    public sealed class PlayerController : MonoBehaviour, FishZone.IChangeFishZone, GameManager.IGameStart,
        PlayerController.IChangeSpeed
    {
        [SerializeField]
        public float speed = 0.25f;

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
        private SpriteRenderer caughtIcon;
        [SerializeField]
        private SplineContainer spline;

        [SerializeField]
        private Sprite fishSprite;
        [SerializeField]
        private Sprite timeSprite;
        [SerializeField]
        private Sprite bootFastSprite;
        [SerializeField]
        private Sprite bootSlowSprite;

        private UniTask? _currentTask;
        private bool     _clickBeginOnPlayer;
        private bool     _insideFishZone;

        private Camera           _camera;
        private FishingMechanics _fishingMechanics;

        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            _camera           = FindFirstObjectByType<Camera>();
            _fishingMechanics = FindFirstObjectByType<FishingMechanics>();

            caughtIcon.transform.localScale = Vector3.zero;

            moveLeftInput.Enable();
            moveRightInput.Enable();
            fishingInput.Enable();
        }

        void GameManager.IGameStart.GameStart()
        {
            CurrentSpeed = speed;
        }

        private void Update()
        {
            var manager = GameManager.Instance;
            if (!manager.Running)
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
                var bounds = manager.gameWorldRect;
                var pos    = transform.position;
                pos.x += CurrentSpeed * moveDirection.Value * Time.deltaTime;
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
                    () => fishingInput.WasPerformedThisFrame() || mouse.leftButton.wasReleasedThisFrame || !manager.Running
                ).ContinueWith(result =>
                {
                    if (!manager.Running)
                    {
                        spriteAnimator.defaultState = "idle";
                        return UniTask.CompletedTask;
                    }

                    caughtIcon.sprite = result.Type switch
                    {
                        FishingSectorView.SectorType.Time  => timeSprite,
                        FishingSectorView.SectorType.Speed => result.Value >= 0 ? bootFastSprite : bootSlowSprite,
                        FishingSectorView.SectorType.Space => null,
                        _                                  => fishSprite
                    };

                    const float show = 0.1f, fly = 0.8f, hide = 0.1f, step = 0.1f;

                    var seq = DOTween.Sequence();
                    seq.Append(caughtIcon.transform.DOScale(1, caughtAnimDur * show));
                    for (var t = 0f; t <= 1f; t += step)
                        seq.Append(caughtIcon.transform.DOMove(spline.EvaluatePosition(t), caughtAnimDur * (fly * step)));
                    seq.Append(caughtIcon.transform.DOScale(0, caughtAnimDur * hide));

                    spriteAnimator.defaultState = "idle";
                    return spriteAnimator.Play("caught", false);
                });
            }
        }

        void FishZone.IChangeFishZone.PlayerEnter() => _insideFishZone = true;
        void FishZone.IChangeFishZone.PlayerExit()  => _insideFishZone = false;

        void IChangeSpeed.PlayerChangeSpeed(int delta) =>
            CurrentSpeed = Mathf.Clamp(CurrentSpeed + delta, speed / 2, 2 * speed);

        public interface ICaughtFish
        {
            void PlayerCaughtFish();
        }

        public interface IChangeSpeed : IOrdered
        {
            void PlayerChangeSpeed(int delta);
        }

        public void OnDamage()
        {
            OnDamageAsync().Forget();
        }

        private async UniTaskVoid OnDamageAsync()
        {
            await DOTween.Sequence()
                .Join(boatSprite.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo))
                .Join(humanSprite.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo));

            GameManager.Instance.Finish(isWin: false);
        }
    }
}