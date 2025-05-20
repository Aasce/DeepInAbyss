using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.Players
{
    public class PlayerInput : MonoBehaviour
    {
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


        public PlayerSettings Settings { get; set; }
        public Camera Camera { get; set; }

        public bool IsPointerOverUI => EventSystem.current && EventSystem.current.IsPointerOverGameObject();
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
            if (Settings == null) return;

            _mousePosition = Camera?.ScreenToWorldPoint(MouseScreenPosition) ?? Vector2.zero;

            _lookInput = Input.GetKey(Settings.LookKey);

            _moveInput = this.MoveAxis();
            _runInput = Input.GetKey(Settings.RunKey);
            _dashInput = Input.GetKey(Settings.DashKey);
            _dodgeInput = Input.GetKey(Settings.DodgeKey);

            _jumpInput = Input.GetAxisRaw("Jump") != 0;
            _crouchInput = Input.GetKeyDown(Settings.CrounchKey);
            _crawlInput = Input.GetKeyDown(Settings.CrawlKey);

            _attackInput = Input.GetKey(Settings.AttackKey);
        }

        private Vector2 MoveAxis()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
    }
}
