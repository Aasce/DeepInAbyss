using UnityEngine;

namespace Asce.Game.Players
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _target;

        [Header("Camera Follower")]
        [SerializeField] private bool _isFollowCamera = true;
        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private Vector2 _offset = Vector2.zero;
        [SerializeField] private float _distanceToIncreaseSpeed = 100f;


        public Camera Camera => _camera;
        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        public bool IsFollowCamera
        {
            get => _isFollowCamera;
            set => _isFollowCamera = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public Vector2 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public float DistanceToIncreaseSpeed
        {
            get => _distanceToIncreaseSpeed;
            set => _distanceToIncreaseSpeed = value;
        }


        private void Update()
        {
            if (Camera == null) return;
            if (Target == null) return;

            if (IsFollowCamera)
            {
                Vector2 targetPosition = (Vector2)Target.position + Offset;
                float distance = Vector2.Distance((Vector2)Camera.transform.position, targetPosition);
                if (distance <= 0.1f) return; 

                float currentSpeed = distance > DistanceToIncreaseSpeed ? Speed * 10f : Speed;

                Vector2 newPosition = Vector2.Lerp(Camera.transform.position, targetPosition, currentSpeed * Time.deltaTime);
                Camera.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.transform.position.z);
            }
        }
    }
}