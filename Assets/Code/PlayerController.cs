using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace JamSpace
{
    public sealed class PlayerController : MonoBehaviour
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
        private SpriteRenderer fishSprite;
        [SerializeField]
        private SplineContainer spline;

        private UniTask? _currentTask;

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
            if (_currentTask.HasValue && !_currentTask.Value.Status.IsCompleted())
                return;

            var mouse             = Mouse.current;
            var worldClickPos     = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
            var wasFishingAsClick = false;
            var moveDirection     = (float?)null;
            if (mouse.leftButton.wasReleasedThisFrame)
                wasFishingAsClick = worldClickPos.DistXY(transform.position) < 1.5f;
            else if (mouse.leftButton.isPressed)
                moveDirection = Math.Clamp(worldClickPos.x - transform.position.x, -1, +1);

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

            if (wasFishingAsClick || fishingInput.WasPerformedThisFrame())
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

        public interface ICaughtFish
        {
            void PlayerCaughtFish(FishingResult result);
        }
    }
}