using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.FloatingTexts;
using Asce.Managers.Utils;
using System;
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

        [Space]
        [SerializeField] private DamageContainer _container;

        public event Action<object, Character> OnCharacterChanged;

        public CameraController CameraController => _cameraController;
        public PlayerSettings Settings => _settings;
        public PlayerInput Input => _input;
        public PlayerUI UI => _ui;

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
                this.ResetControlCharacter();
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

            Character.Action.Looking(Input.LookInput, Input.MousePosition);

            Character.Action.Moving(Input.MoveInput);
            Character.Action.Running(Input.RunInput);

            if (Input.DashInput) Character.Action.Dashing();
            if (Input.DodgeInput) Character.Action.Dodging();

            Character.Action.Jumping(Input.JumpInput);
            if (Input.CrouchInput) Character.Action.Crouching();
            if (Input.CrawlInput) Character.Action.Crawling();
        }

        private void ControlUI()
        {

        }

        public void SetCharacter(Character character)
        {
            if (character == null) return;

            _character = character;
            OnCharacterChanged?.Invoke(this, Character);
        }

        private void ResetControlCharacter()
        {
            if (Character == null) return;
            Character.Action.Looking(false, Character.transform.position);

            Character.Action.Moving(Vector2.zero);
            Character.Action.Running(false);
            Character.Action.Jumping(false);
        }
    }
}
