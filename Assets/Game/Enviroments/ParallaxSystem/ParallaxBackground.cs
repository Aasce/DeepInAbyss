using Asce.Game.Players;
using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    /// <summary>
    ///     Makes background move slower than camera to create parallax effect.
    ///     Attach this to each background layer.
    /// </summary>
    public class ParallaxBackground : MonoBehaviour
    {
        [Tooltip("How much this layer moves relative to camera. 0 = static, 1 = same speed as camera.")]
        [SerializeField,Range(0f, 1f)] protected float _parallaxFactor = 0.5f;
        [SerializeField,Range(0f, 1f)] protected float _parallaxFactorY = 1f;
        
        [SerializeField, Readonly] private Vector2 _startPosition;
        [SerializeField] protected Vector2 _startOffset = Vector2.zero;
        [SerializeField] protected Vector2 _offset = Vector2.zero;

        private Transform _camera;

        public Vector2 StartPosition => _startPosition + _startOffset;

        private void Start()
        {
            _camera = Player.Instance.CameraController.Camera.transform;
            if (_camera == null) _startPosition = transform.position;
            else _startPosition = transform.position - _camera.position;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;

            Vector2 delta = (Vector2)_camera.position - StartPosition;
            transform.position = (Vector3)StartPosition + (Vector3)_offset + new Vector3(
                delta.x * _parallaxFactor,
                delta.y * _parallaxFactorY,
                transform.position.z
            );
        }
    }
}
