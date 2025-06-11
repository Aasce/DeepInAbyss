using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Players
{
    public class Player : MonoBehaviourSingleton<Player>
    {
        [SerializeField, HideInInspector] private CameraController _cameraController;
        [SerializeField, HideInInspector] private PlayerSettings _settings;
        [SerializeField, HideInInspector] private PlayerInput _input;
        [SerializeField, HideInInspector] private PlayerUI _ui;

        [Space]
        [SerializeField] private Character _mainCharacter;
        [SerializeField] private ICreature _controlledCreature;

        public event Action<object, ValueChangedEventArgs<ICreature>> OnControlledCreatureChanged;

        public CameraController CameraController => _cameraController;
        public PlayerSettings Settings => _settings;
        public PlayerInput Input => _input;
        public PlayerUI UI => _ui;

        public ICreature ControlledCreature => _controlledCreature;

        private void Reset()
        {
            this.LoadComponent(out _cameraController);
            if (this.LoadComponent(out _settings)) _settings.Player = this;
            if (this.LoadComponent(out _input)) _input.Player = this;
            if (this.LoadComponent(out _ui)) _ui.Player = this;
        }

        private void Start()
        {
            if (_mainCharacter != null) this.SetControlledCreature(_mainCharacter);
            
            if (CameraController == null) return;
            if (ControlledCreature == null) return;

            CameraController.Target = ControlledCreature.gameObject.transform;
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
            if (ControlledCreature == null) return;

            if (ControlledCreature.Action is ILookable lookable) lookable.Looking(Input.LookInput, Input.MousePosition);

            if (ControlledCreature.Action is IMovable movable) movable.Moving(Input.MoveInput);
            if (ControlledCreature.Action is IRunnable runnable) runnable.Running(Input.RunInput);

            if (Input.DashInput) if (ControlledCreature.Action is IDashable dashable) dashable.Dashing();
            if (Input.DodgeInput) if (ControlledCreature.Action is IDodgeable dodgeable) dodgeable.Dodging();

            if (ControlledCreature.Action is IJumpable jumpable) jumpable.Jumping(Input.JumpInput);
            if (Input.CrouchInput) if (ControlledCreature.Action is ICrouchable crouchable) crouchable.Crouching();
            if (Input.CrawlInput) if (ControlledCreature.Action is ICrawlable crawlable) crawlable.Crawling();
        }

        private void ControlUI()
        {

        }

        public void SetControlledCreature(ICreature creature)
        {
            if (creature == null) return;
            ICreature oldControlledCreature = ControlledCreature;
            _controlledCreature = creature;

            oldControlledCreature.UncontrolledByPlayer();
            ControlledCreature.ControlledByPlayer();

            CameraController.Target = ControlledCreature.gameObject.transform;
            OnControlledCreatureChanged?.Invoke(this, new ValueChangedEventArgs<ICreature>(oldControlledCreature, ControlledCreature));
        }

        private void ResetControlCharacter()
        {
            if (ControlledCreature == null) return;
            
            if (ControlledCreature.Action is ILookable lookable) lookable.Looking(false, ControlledCreature.gameObject.transform.position);
            if (ControlledCreature.Action is IMovable movable) movable.Moving(Vector2.zero);
            if (ControlledCreature.Action is IRunnable runnable) runnable.Running(false);
            if (ControlledCreature.Action is IJumpable jumpable) jumpable.Jumping(false);
        }
    }
}
