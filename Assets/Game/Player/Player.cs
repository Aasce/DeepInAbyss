using Asce.Game.Entities;
using Asce.Game.Entities.Characters;
using Asce.Game.Enviroments;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Players
{
    public class Player : MonoBehaviourSingleton<Player>
    {
        [SerializeField, Readonly] private CameraController _cameraController;
        [SerializeField, Readonly] private PlayerSettings _settings;
        [SerializeField, Readonly] private PlayerInput _input;
        [SerializeField, Readonly] private PlayerUI _ui;

        [Space]
        [SerializeField] private Character _mainCharacter;
        [SerializeField] private ICreature _controlledCreature;

        public event Action<object, ValueChangedEventArgs<ICreature>> OnControlledCreatureChanged;

        public CameraController CameraController => _cameraController;
        public PlayerSettings Settings => _settings;
        public PlayerInput Input => _input;
        public PlayerUI UI => _ui;

        public Character MainCharacter => _mainCharacter;
        public ICreature ControlledCreature => _controlledCreature;

        protected override void RefReset()
        {
            base.RefReset();
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

            CameraController.ToTarget(Vector2.up * 10f);
        }

        private void Update()
        {
            this.ControlUI();
            if (Input.IsControlUI)
            {
                this.ResetControl();
            }
            else
            {
                this.FocusInteractableObject();
                this.InteractWithObject();
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

            if (ControlledCreature.Action is IAttackable attackable)
            {
                attackable.Attacking(Input.AttackInput);
                attackable.MeleeAttacking(Input.MeleeAttackInput);
            }
        }

        private void ControlUI()
        {
            if (Input.ToggleInventoryInput)
            {
                UI.ToggleInventory();
            }

        }

        public void SetControlledCreature(ICreature creature)
        {
            if (creature == null) return;
            ICreature oldControlledCreature = ControlledCreature;

            this.CreatureUnregister();
            _controlledCreature = creature;
            this.CreatureRegister();

            OnControlledCreatureChanged?.Invoke(this, new ValueChangedEventArgs<ICreature>(oldControlledCreature, ControlledCreature));
        }

        private void ResetControl()
        {
            if (ControlledCreature == null) return;

            if (ControlledCreature.Action is ILookable lookable) lookable.Looking(false, ControlledCreature.gameObject.transform.position);
            if (ControlledCreature.Action is IMovable movable) movable.Moving(Vector2.zero);
            if (ControlledCreature.Action is IRunnable runnable) runnable.Running(false);
            if (ControlledCreature.Action is IJumpable jumpable) jumpable.Jumping(false);
        }

        private void FocusInteractableObject()
        {
            if (ControlledCreature is not Character character) return;
            if (character.Interaction == null) return;
            character.Interaction.FocusAt(Input.MousePosition);
        }

        private void InteractWithObject()
        {
            if (!Input.InteractionInput) return;
            if (ControlledCreature is not Character character) return;
            if (character.Interaction == null) return;

            IInteractableObject focusObject = character.Interaction.FocusObject;
            if (focusObject == null) return;

            focusObject.Interact(character.gameObject);
        }

        private void CreatureRegister()
        {
            if (ControlledCreature == null) return;

            CameraController.Target = ControlledCreature.gameObject.transform;
            ControlledCreature.ControlledByPlayer();
        }

        private void CreatureUnregister()
        {
            if (ControlledCreature == null) return;

            this.ResetControl();
            ControlledCreature.UncontrolledByPlayer();
        }
    }
}
