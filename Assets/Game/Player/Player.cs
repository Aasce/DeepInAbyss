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

        [Space]
        [SerializeField] private DamageContainer _container;


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

            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
            {
                Character.Stats.HealthGroup.HealScale.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat, 10f);
                StatValuePopupManager.Instance.CreateValuePopup($"+1 Heal Scale", Color.green, size: 40f, Character.transform.position);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.N))
            {
                Character.Stats.Speed.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat, 10f);
                StatValuePopupManager.Instance.CreateValuePopup($"+1 Speed", Color.blue, size: 40f, Character.transform.position);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                Character.Stats.JumpForce.AddAgent(gameObject, "Player buff", 1f, Stats.StatValueType.Plat, 10f);
                StatValuePopupManager.Instance.CreateValuePopup($"+1 Jump Force", Color.blue, size: 40f, Character.transform.position);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.K))
            {
                if (Character.Status.IsAlive)
                {
                    Character.Status.SetStatus(EntityStatusType.Dead);
                    StatValuePopupManager.Instance.CreateValuePopup($"Kill", Color.red, size: 40f, Character.transform.position);
                }
                else if (Character.Status.IsDead)
                {
                    Character.Status.SetStatus(EntityStatusType.Alive);
                    StatValuePopupManager.Instance.CreateValuePopup($"Revive", Color.yellow, size: 40f, Character.transform.position);
                }
            }

            if ( UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        _container = new(Character.Stats, creature.Stats)
                        {
                            Damage = Character.Stats.Strength.Value,
                            DamageType = DamageType.Physical,
                        };
                        CombatSystem.DamageDealing(_container);

                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        _container = new(creature.Stats, Character.Stats)
                        {
                            Damage = creature.Stats.Strength.Value,
                            DamageType = DamageType.Magical,
                        };
                        CombatSystem.DamageDealing(_container);
                    }
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        _container = new(Character.Stats, creature.Stats)
                        {
                            Damage = creature.Stats.HealthGroup.Health.Value * 0.25f,
                            DamageType = DamageType.TrueDamage,
                        };
                        CombatSystem.DamageDealing(_container);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        CombatSystem.Healing(Character.Stats, creature.Stats, creature.transform.position, 20f);
                    }
                }
            }


            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (Input.IsMouseHit(out RaycastHit2D hit))
                {
                    if (hit.transform.TryGetComponent(out Creature creature))
                    {
                        creature.Stats.HealthGroup.Health.AddAgent(gameObject, "add health", 100f, Stats.StatValueType.Plat, duration: 10f);
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
                        StatValuePopupManager.Instance.CreateValuePopup($"+1% Strength", Color.magenta, size: 30f, creature.transform.position);
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
