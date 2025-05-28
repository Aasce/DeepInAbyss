using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerSettings _settings;
        [SerializeField] private PlayerInput _input;

        [SerializeField] private Character _character;

        public CameraController CameraController => _cameraController;
        public PlayerSettings Settings => _settings;
        public PlayerInput Input => _input;

        public Character Character => _character;

        private void Awake()
        {
            if (Input != null)
            {
                Input.Settings = Settings;
                Input.Camera = _cameraController.Camera;
            }
        }

        private void Start()
        {
            if (CameraController == null) return;
            if (Character == null) return;
            CameraController.Target = Character.transform;
        }

        private void Update()
        {
            if (Input.IsControlUI)
            {
                this.ControlUI();
            }
            else
            {
                this.ControlCharacter();
            }


        }

        private void ControlCharacter()
        {
            if (Character == null) return;

            if (Input.IsPointerOverUI)
            {

            }
            else
            {
                Character.Action.ControlLooking(Input.LookInput, Input.MousePosition);

                Character.Action.ControlMoving(Input.MoveInput, Input.RunInput);
                Character.Action.ControlDashing(Input.DashInput);
                Character.Action.ControlDodging(Input.DodgeInput);

                Character.Action.ControlJumping(Input.JumpInput);
                Character.Action.ControlCrouching(Input.CrouchInput);
                Character.Action.ControlCrawling(Input.CrawlInput);
            }
        }

        private void ControlUI()
        {

        }
    }
}
