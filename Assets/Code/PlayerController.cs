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
        private const float StepLength   = 4f;
        private const int   CountOfSteps = 5;

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

        public Vector2Int GridPos => new (
            Mathf.RoundToInt(transform.position.x / StepLength),
            Mathf.RoundToInt(transform.position.y / StepLength)
        );
        public int GridX => GridPos.x;

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

            var mouse         = Mouse.current;
            var worldClickPos = (Vector3?)null;
            if (mouse.leftButton.wasReleasedThisFrame)
                worldClickPos = _camera.ScreenToWorldPoint(mouse.position.ReadValue());

            var wasFishingAsClick = false;
            var moveStep          = (int?)null;
            if (worldClickPos.HasValue)
            {
                var clickGridPos = new Vector2Int(
                    Mathf.RoundToInt(worldClickPos.Value.x / StepLength),
                    Mathf.RoundToInt(worldClickPos.Value.y / StepLength)
                );
                if (clickGridPos.y is 0)
                {
                    wasFishingAsClick = clickGridPos == GridPos;
                    if (!wasFishingAsClick)
                        moveStep = clickGridPos.x - GridPos.x;
                }
            }
            if (moveLeftInput.WasPerformedThisFrame())
                moveStep = -1;
            if (moveRightInput.WasPerformedThisFrame())
                moveStep = +1;

            if (moveStep.HasValue)
            {
                _currentTask = MoveAsync(moveStep.Value);
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

                    spriteAnimator.defaultState = "idle";
                    return spriteAnimator.Play("caught", false);
                });
            }
        }

        private UniTask MoveAsync(float step)
        {
            const float endX = (CountOfSteps - 1) / 2f;

            var currentX = GridX;
            if (currentX + step > endX)
                step = endX - currentX;
            if (currentX + step < -endX)
                step = -endX - currentX;

            step *= StepLength;
            return step is not 0
                ? transform.DOMoveX(transform.position.x + step, Math.Abs(step) * speed).ToUniTask()
                : UniTask.CompletedTask;
        }
    }
}