using Asce.Game.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asce.Game.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Character _character;

        public CameraController CameraController => _cameraController;
        public Character Character => _character;

        private void Start()
        {
            if (CameraController == null) return;
            if (Character == null) return;
            CameraController.Target = Character.transform;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnJumpPerformed += OnJump;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnJumpPerformed -= OnJump;
        }

        private void FixedUpdate()
        {
            this.ControlCharacter();
        }

        private void ControlCharacter()
        {
            if (Character == null) return;
            Character.Move(InputManager.Instance.MoveAxis);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (Character == null) return;
            Character.Jump();
        }
    }
}
