using System;
using UnityEngine;

namespace Code
{
    /// <summary>
    /// Fit by any dimension and expand by the other, work now only for Orthographic
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraWorldExpand : MonoBehaviour
    {
        [SerializeField]
        private float minWorldWidth = 10f;
        [SerializeField]
        private float minWorldHeight = 5f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!_camera.orthographic)
                Debug.LogWarning("Wrong camera type! Set camera to orthographic!");
            _camera.orthographicSize = Math.Max(minWorldWidth / _camera.aspect, minWorldHeight);
        }
    }
}