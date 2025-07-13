using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Equipments.Weapons;
using Asce.Game.FloatingTexts;
using Asce.Game.Stats;
using Asce.Game.StatusEffects;
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
            CameraController.ToTarget(Vector2.up * 10f);
        }

        private void Update()
        {
            this.ControlUI();
            if (Input.IsControlUI)
            {
                this.ResetControlCharacter();
            }
            else
            {
                this.ControlCharacter();
            }

            this.Test();
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

            if (Input.DetachWeaponInput) if (ControlledCreature.Equipment is IHasWeaponSlot hasWeaponSlot) hasWeaponSlot.WeaponSlot.DetachWeapon();
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
            this.ResetControlCharacter();

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


        private void Test()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        SetControlledCreature(creature);
                    }
                }
            }

            if (ControlledCreature == null) return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                if (ControlledCreature.Equipment is IHasWeaponSlot hasWeaponSlot)
                {
                    Weapon weapon = null;
                    Weapon currentWeapon = hasWeaponSlot.WeaponSlot.CurrentWeapon;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(ControlledCreature.gameObject.transform.position, 1f);
                    foreach (Collider2D collider in colliders)
                    {
                        if (currentWeapon != null && currentWeapon.transform == collider.transform) continue;
                        if (!collider.TryGetComponent(out Weapon weaponCollider)) continue;

                        weapon = weaponCollider;
                        break;
                    }

                    if (weapon != null)
                    {
                        if (currentWeapon != null) hasWeaponSlot.WeaponSlot.DetachWeapon();
                        hasWeaponSlot.WeaponSlot.AddWeapon(weapon);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.HealthGroup.HealScale.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat);
                        StatValuePopupManager.Instance.CreateValuePopup($"+1 Heal Scale", Color.green, size: 40f, creature.transform.position);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.N))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.Speed.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat);
                        StatValuePopupManager.Instance.CreateValuePopup($"+1 Speed", Color.blue, size: 40f, creature.transform.position);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                if (ControlledCreature.Stats is IHasJumpForce hasJumpForce)
                {
                    hasJumpForce.JumpForce.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat);
                    StatValuePopupManager.Instance.CreateValuePopup($"+1 Jump Force", Color.blue, size: 40f, ControlledCreature.gameObject.transform.position);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        StatusEffectsManager.Instance.SendEffect<Ignite_StatusEffect>(ControlledCreature as Creature, creature, new EffectDataContainer()
                        {
                            Duration = 4f,
                            Strength = 10f,
                        });
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.O))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        StatusEffectsManager.Instance.SendEffect<Weakened_StatusEffect>(ControlledCreature as Creature, creature, new EffectDataContainer()
                        {
                            Duration = 5f,
                            Strength = 0.25f,
                        });
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.I))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        StatusEffectsManager.Instance.SendEffect<Freeze_StatusEffect>(ControlledCreature as Creature, creature, new EffectDataContainer()
                        {
                            Duration = 5f,
                            Strength = 0.5f,
                        });
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.K))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        if (creature.Status.IsAlive)
                        {
                            creature.Status.SetStatus(EntityStatusType.Dead);
                            StatValuePopupManager.Instance.CreateValuePopup($"Kill", Color.red, size: 40f, creature.transform.position);
                        }
                        else if (creature.Status.IsDead)
                        {
                            creature.Status.SetStatus(EntityStatusType.Alive);
                            StatValuePopupManager.Instance.CreateValuePopup($"Revive", Color.yellow, size: 40f, creature.transform.position);
                        }
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.J))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        float breathe = creature.Stats.SustenanceGroup.Breath.ChangeStat.Value;
                        StatAgent underwaterAgent = creature.Stats.SustenanceGroup.Breath.FindAgents(gameObject, "underwater");
                        if (underwaterAgent == null)
                        {
                            creature.Stats.SustenanceGroup.Breath.ChangeStat.AddAgent(gameObject, "underwater", -(breathe + 2f));
                            StatValuePopupManager.Instance.CreateValuePopup($"Underwater", Color.magenta, size: 40f, creature.transform.position);
                        }
                        else
                        {
                            creature.Stats.SustenanceGroup.Breath.ChangeStat.RemoveAllAgents(gameObject, "underwater");
                            StatValuePopupManager.Instance.CreateValuePopup($"Unapply Underwater", Color.magenta, size: 40f, creature.transform.position);
                        }
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        StatusEffectsManager.Instance.SendEffect<Invisibility_StatusEffect>(ControlledCreature as Creature, creature, new EffectDataContainer()
                        {
                            Duration = 4f,
                            Strength = 0.4f,
                        });
                        StatValuePopupManager.Instance.CreateValuePopup($"Invisible", Color.gray, size: 40f, creature.transform.position);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.G))
            {
                if (ControlledCreature.Action is IThrowableWeapon throwable)
                {
                    throwable.Throwing();
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        DamageContainer container = new(ControlledCreature.Stats, creature.Stats)
                        {
                            Damage = ControlledCreature.Stats.Strength.Value,
                            DamageType = DamageType.Physical,
                        };
                        CombatSystem.DamageDealing(container);

                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        DamageContainer container = new(ControlledCreature.Stats, creature.Stats)
                        {
                            Damage = creature.Stats.Strength.Value,
                            DamageType = DamageType.Magical,
                        };
                        CombatSystem.DamageDealing(container);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        DamageContainer container = new(ControlledCreature.Stats, creature.Stats)
                        {
                            Damage = creature.Stats.HealthGroup.Health.Value * 0.25f,
                            DamageType = DamageType.TrueDamage,
                        };
                        CombatSystem.DamageDealing(container);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        CombatSystem.Healing(ControlledCreature, creature.Stats, creature.transform.position, 20f);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.HealthGroup.Health.AddAgent(gameObject, "add health", 100f, Stats.StatValueType.Plat);
                        StatValuePopupManager.Instance.CreateValuePopup($"+100 Max Health", Color.green, size: 30f, creature.transform.position);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.Strength.AddAgent(gameObject, "add strength", 1f, Stats.StatValueType.Ratio);
                        StatValuePopupManager.Instance.CreateValuePopup($"+100% Strength", Color.magenta, size: 30f, creature.transform.position);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.DefenseGroup.Armor.AddAgent(gameObject, "add armor", 10f, Stats.StatValueType.Plat);
                        StatValuePopupManager.Instance.CreateValuePopup($"+10 Armor", Color.yellow, size: 30f, creature.transform.position);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.DefenseGroup.Shield.AddAgent(gameObject, "add Shield", 100f, Stats.StatValueType.Plat);
                        StatValuePopupManager.Instance.CreateValuePopup($"+100 Shield", Color.yellow, size: 30f, creature.transform.position);
                    }
                }
            }

            int dropIndex = -1;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad0)) dropIndex = 0;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad1)) dropIndex = 1;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad2)) dropIndex = 2;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad3)) dropIndex = 3;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad4)) dropIndex = 4;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad5)) dropIndex = 5;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad6)) dropIndex = 6;

            if (dropIndex >= 0)
            {
                ControlledCreature.Inventory.Drop(dropIndex);
            }
        }
    }
}
