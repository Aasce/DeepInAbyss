using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.Players
{
    public class PlayerInput : MonoBehaviour, IPlayerComponent
    {
        // Ref
        [SerializeField, HideInInspector] private Player _player;

        [SerializeField] private Vector2 _mousePosition;

        [Space]
        [SerializeField] private bool _lookInput;

        [Space]
        [SerializeField] private Vector2 _moveInput;
        [SerializeField] private bool _runInput;
        [SerializeField] private bool _dashInput;
        [SerializeField] private bool _dodgeInput;

        [SerializeField] private bool _jumpInput;
        [SerializeField] private bool _crouchInput;
        [SerializeField] private bool _crawlInput;
        
        [SerializeField] private bool _attackInput;

        [Space]
        [SerializeField] private bool _isControlUI = false;


        public Player Player
        {
            get => _player;
            set => _player = value;
        }

        public bool IsPointerOverUI => Player != null && Player.UI.IsPointerOverScreenSpaceUI(MouseScreenPosition);
        public Vector2 MouseScreenPosition => Input.mousePosition;
        public Vector2 MousePosition => _mousePosition;

        public bool LookInput => _lookInput;

        public Vector2 MoveInput => _moveInput;
        public bool RunInput => _runInput;
        public bool DashInput => _dashInput;
        public bool DodgeInput => _dodgeInput;

        public bool JumpInput => _jumpInput;
        public bool CrouchInput => _crouchInput;
        public bool CrawlInput => _crawlInput; 
        public bool AttackInput => _attackInput;

        public bool IsControlUI
        {
            get => _isControlUI;
            set => _isControlUI = value;
        }


        private void Update()
        {
            if (Player.Settings == null) return;
            if (Player.CameraController.Camera == null) return;

            _mousePosition = Player.CameraController.Camera.ScreenToWorldPoint(MouseScreenPosition);

            _lookInput = Input.GetKey(Player.Settings.LookKey);

            _moveInput = this.MoveAxis();
            _runInput = Input.GetKey(Player.Settings.RunKey);
            _dashInput = Input.GetKey(Player.Settings.DashKey);
            _dodgeInput = Input.GetKey(Player.Settings.DodgeKey);

            _jumpInput = Input.GetAxisRaw("Jump") != 0;
            _crouchInput = Input.GetKeyDown(Player.Settings.CrounchKey);
            _crawlInput = Input.GetKeyDown(Player.Settings.CrawlKey);

            _attackInput = Input.GetKey(Player.Settings.AttackKey);
        }

        private Vector2 MoveAxis()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        public bool IsMouseHit(out RaycastHit2D hit)
        {
            hit = Physics2D.Raycast(MousePosition, Vector2.zero, 0f, Player.Settings.MouseLayerMask);
            return hit.collider != null;
        }
    }
}
