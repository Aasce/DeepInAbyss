using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.FloatingTexts;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CameraController _cameraController;
        [SerializeField, HideInInspector] private PlayerSettings _settings;
        [SerializeField, HideInInspector] private PlayerInput _input;
        [SerializeField, HideInInspector] private PlayerUI _ui;

        [Space]
        [SerializeField] private Character _character;

        public CameraController CameraController => _cameraController;
        public PlayerSettings Settings => _settings;
        public PlayerInput Input => _input;
        public PlayerUI InputUI => _ui;

        public Character Character => _character;

        private void Reset()
        {
            this.LoadComponent(out _cameraController);
            if (this.LoadComponent(out _settings)) _settings.Player = this;
            if (this.LoadComponent(out _input)) _input.Player = this;
            if (this.LoadComponent(out _ui)) _ui.Player = this;
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
