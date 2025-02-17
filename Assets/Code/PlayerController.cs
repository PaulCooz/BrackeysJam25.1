using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public int GridX => Mathf.RoundToInt(transform.position.x / StepLength);

        private UniTask? _currentTask;

        private FishingMechanics _fishingMechanics;

        private void Awake()
        {
            _fishingMechanics = FindFirstObjectByType<FishingMechanics>();

            moveLeftInput.Enable();
            moveRightInput.Enable();
            fishingInput.Enable();
        }

        private void Update()
        {
            if (_currentTask.HasValue && !_currentTask.Value.Status.IsCompleted())
                return;

            if (moveLeftInput.WasPerformedThisFrame())
            {
                _currentTask = MoveAsync(-StepLength);
            }
            if (moveRightInput.WasPerformedThisFrame())
            {
                _currentTask = MoveAsync(+StepLength);
            }
            if (fishingInput.WasPerformedThisFrame())
            {
                _currentTask = _fishingMechanics.Run(() => fishingInput.WasPerformedThisFrame());
            }
        }

        private UniTask MoveAsync(float step)
        {
            const float endX = (CountOfSteps - 1) * StepLength / 2f;

            var currentX = GridX * StepLength;
            if (currentX + step > endX)
                step = endX - currentX;
            if (currentX + step < -endX)
                step = -endX - currentX;

            return step is not 0
                ? transform.DOMoveX(transform.position.x + step, Math.Abs(step) * speed).ToUniTask()
                : UniTask.CompletedTask;
        }
    }
}