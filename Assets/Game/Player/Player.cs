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
                Character.PhysicController.ControlLooking(Input.LookInput, Input.MousePosition);

                Character.PhysicController.ControlMoving(Input.MoveInput, Input.RunInput);
                Character.PhysicController.ControlDashing(Input.DashInput);
                Character.PhysicController.ControlDodging(Input.DodgeInput);

                Character.PhysicController.ControlJumping(Input.JumpInput);
                Character.PhysicController.ControlCrouching(Input.CrouchInput);
                Character.PhysicController.ControlCrawling(Input.CrawlInput);
            }
        }

        private void ControlUI()
        {

        }
    }
}
