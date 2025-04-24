using Asce.Managers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asce.Game.Players
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private InputActionAsset _inputActionAsset;

        private InputActionMap _playerActionMap;


        public event Action<InputAction.CallbackContext> OnJumpPerformed;
        public event Action<InputAction.CallbackContext> OnCrouchPerformed;
        public event Action<InputAction.CallbackContext> OnUseItem1Performed;
        public event Action<InputAction.CallbackContext> OnUseItem2Performed;
        public event Action<InputAction.CallbackContext> OnOpenBagPerformed;
        public event Action<InputAction.CallbackContext> OnOpenMapPerformed;
        public event Action<InputAction.CallbackContext> OnPausePerformed;

        public Vector2 MoveAxis => MoveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool IsMoving => MoveAxis.sqrMagnitude > 0.01f;


        public InputAction MoveAction { get; private set; }
        public InputAction CrouchAction { get; private set; }
        public InputAction JumpAction { get; private set; }
        public InputAction UseItem1Action { get; private set; }
        public InputAction UseItem2Action { get; private set; }
        public InputAction OpenBagAction { get; private set; }
        public InputAction OpenMapAction { get; private set; }
        public InputAction PauseAction { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            _playerActionMap = _inputActionAsset.FindActionMap("Player", true);

            MoveAction = _playerActionMap.FindAction("Move", true);
            CrouchAction = _playerActionMap.FindAction("Crouch", true);
            JumpAction = _playerActionMap.FindAction("Jump", true);
            UseItem1Action = _playerActionMap.FindAction("UseItem1", true);
            UseItem2Action = _playerActionMap.FindAction("UseItem2", true);
            OpenBagAction = _playerActionMap.FindAction("OpenBag", true);
            OpenMapAction = _playerActionMap.FindAction("OpenMap", true);
            PauseAction = _playerActionMap.FindAction("Pause", true);
        }

        private void OnEnable()
        {
            _playerActionMap.Enable();

            JumpAction.performed += HandleJump;
            CrouchAction.performed += HandleCrouch;
            UseItem1Action.performed += HandleUseItem1;
            UseItem2Action.performed += HandleUseItem2;
            OpenBagAction.performed += HandleOpenBag;
            OpenMapAction.performed += HandleOpenMap;
            PauseAction.performed += HandlePause;
        }

        private void OnDisable()
        {
            _playerActionMap.Disable();

            JumpAction.performed -= HandleJump;
            CrouchAction.performed -= HandleCrouch;
            UseItem1Action.performed -= HandleUseItem1;
            UseItem2Action.performed -= HandleUseItem2;
            OpenBagAction.performed -= HandleOpenBag;
            OpenMapAction.performed -= HandleOpenMap;
            PauseAction.performed -= HandlePause;
        }

        private void HandleJump(InputAction.CallbackContext ctx) => OnJumpPerformed?.Invoke(ctx);
        private void HandleCrouch(InputAction.CallbackContext ctx) => OnCrouchPerformed?.Invoke(ctx);
        private void HandleUseItem1(InputAction.CallbackContext ctx) => OnUseItem1Performed?.Invoke(ctx);
        private void HandleUseItem2(InputAction.CallbackContext ctx) => OnUseItem2Performed?.Invoke(ctx);
        private void HandleOpenBag(InputAction.CallbackContext ctx) => OnOpenBagPerformed?.Invoke(ctx);
        private void HandleOpenMap(InputAction.CallbackContext ctx) => OnOpenMapPerformed?.Invoke(ctx);
        private void HandlePause(InputAction.CallbackContext ctx) => OnPausePerformed?.Invoke(ctx);
    }
}