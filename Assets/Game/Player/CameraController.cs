using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Players
{
    /// <summary>
    ///     Controls the camera to follow a target (usually the character) in game.
    /// </summary>
    public class CameraController : GameComponent
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Camera _renderCamera;

        [Tooltip("The target (usually the player) to follow")]
        [SerializeField] private Transform _target;

        [Header("Camera Follower")]
        [Tooltip("Toggle whether the camera follows the target")]
        [SerializeField] private bool _isFollowCamera = true;

        [Tooltip("Base follow speed")]
        [SerializeField] private float _speed = 1.0f;

        [Tooltip("Positional offset from the target")]
        [SerializeField] private Vector2 _offset = Vector2.zero;

        [Tooltip("Distance threshold to boost camera speed")]
        [SerializeField] private float _distanceToIncreaseSpeed = 100f;

        [Header("Render Creature")]
        [SerializeField] private bool _isActiveRenderCamera = true;

        /// <summary>
        ///     The camera this controller is managing.
        /// </summary>
        public Camera Camera => _camera;
        public Camera RenderCamera => _renderCamera;

        /// <summary>
        ///     The target the camera should follow.
        /// </summary>
        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        /// <summary>
        ///     Determines whether the camera should currently follow the target.
        /// </summary>
        public bool IsFollowCamera
        {
            get => _isFollowCamera;
            set => _isFollowCamera = value;
        }

        /// <summary>
        ///     The normal speed at which the camera follows the target.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        /// <summary>
        ///     The positional offset applied to the camera relative to the target.
        /// </summary>
        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        /// <summary>
        ///     The distance threshold beyond which the camera follows the target at an accelerated speed.
        /// </summary>
        public float DistanceToIncreaseSpeed
        {
            get => _distanceToIncreaseSpeed;
            set => _distanceToIncreaseSpeed = value;
        }

        public bool IsActiveRenderCamera
        {
            get => _isActiveRenderCamera;
            set => _isActiveRenderCamera = value;
        }

        private void Update()
        {
            if (Camera == null) return;
            if (Target == null) return;

            if (IsFollowCamera)
            {
                Vector2 targetPosition = (Vector2)Target.position + Offset;
                float distance = Vector2.Distance((Vector2)Camera.transform.position, targetPosition);
                if (distance <= 0.1f) return; // If distance is very small, skip movement to avoid jitter

                // Boost speed if distance exceeds threshold
                float currentSpeed = distance > DistanceToIncreaseSpeed ? Speed * 10f : Speed;

                // Interpolate smoothly to the target position
                Vector2 newPosition = Vector2.Lerp(Camera.transform.position, targetPosition, currentSpeed * Time.deltaTime);
                Camera.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.transform.position.z);
            }
        }

        private void LateUpdate()
        {
            if (RenderCamera == null) return;

            if (!_isActiveRenderCamera)
            {
                RenderCamera.gameObject.SetActive(false); // always disable if not active render camera
                return;
            }

            if (Player.Instance.ControlledCreature == null)
            {
                RenderCamera.gameObject.SetActive(false); // disable because no target to follow
                return;
            }

            RenderCamera.gameObject.SetActive(true); // enable camera

            // update camera position
            Vector2 position = Player.Instance.ControlledCreature.gameObject.transform.position;
            float height = Player.Instance.ControlledCreature.Status.Height;
            RenderCamera.transform.position = new Vector3(position.x, position.y + height * 0.5f, RenderCamera.transform.position.z);

        }

        /// <summary>
        ///     Set Camera position to Target position
        /// </summary>
        /// <param name="offset"></param>
        public void ToTarget(Vector2 offset = default)
        {
            if (Camera == null) return;
            if (Target == null) return;

            float x = Target.position.x + offset.x;
            float y = Target.position.y + offset.y;
            Camera.transform.position = new Vector3(x, y, Camera.transform.position.z);
        }
    }
}