using Asce.Game.Combats;
using Asce.Game.SaveLoads;
using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, ICreature, IOptimizedComponent, IHasPhysicController<CreaturePhysicController>,
        IHasView<CreatureView>, IHasUI<CreatureUI>, IHasAction<CreatureAction>, 
        IHasStats<CreatureStats, SO_CreatureBaseStats>, IHasStatusEffect<CreatureStatusEffect>, ISendDamageable, ITakeDamageable,
        IHasEquipment<CreatureEquipment>, IHasInventory<CreatureInventory>, IHasSpoils<CreatureSpoils>
    {
        [SerializeField, Readonly] private CreaturePhysicController _physicController;
        [SerializeField, Readonly] private CreatureAction _action;
        [SerializeField, Readonly] private CreatureView _view;
        [SerializeField, Readonly] private CreatureUI _ui;
        [SerializeField, Readonly] private CreatureStats _stats;
        [SerializeField, Readonly] private CreatureStatusEffect _statusEffect;
        [SerializeField, Readonly] private CreatureEquipment _equipment;
        [SerializeField, Readonly] private CreatureInventory _inventory;
        [SerializeField, Readonly] private CreatureSpoils _spoils;

        private bool _isControled = false;

        public event Action<object, DamageContainer> OnBeforeSendDamage;
        public event Action<object, DamageContainer> OnAfterSendDamage;
        public event Action<object, DamageContainer> OnBeforeTakeDamage;
        public event Action<object, DamageContainer> OnAfterTakeDamage;

        public virtual bool IsDead => Status.IsDead;

        public CreaturePhysicController PhysicController
        {
            get => _physicController;
            set => _physicController = value;
        }

        public CreatureAction Action
        {
            get => _action; 
            set => _action = value;
        }

        public CreatureView View
        {
            get => _view;
            set => _view = value;
        }

        public CreatureUI UI
        {
            get => _ui;
            set => _ui = value;
        }

        public CreatureStats Stats
        {
            get => _stats;
            set => _stats = value;
        }

        public CreatureStatusEffect StatusEffect
        {
            get => _statusEffect;
            set => _statusEffect = value;
        }

        public CreatureEquipment Equipment
        {
            get => _equipment;
            set => _equipment = value;
        }

        public CreatureInventory Inventory
        {
            get => _inventory;
            set => _inventory = value;
        }

        public CreatureSpoils Spoils
        {
            get => _spoils;
            set => _spoils = value;
        }

        public bool IsControled
        {
            get => _isControled;
            set => _isControled = value;
        }


        protected override void RefReset()
        {
            base.RefReset();
            if (this.LoadComponent(out _physicController)) PhysicController.Owner = this;
            if (this.LoadComponent(out _view)) View.Owner = this;
            if (this.LoadComponent(out _ui)) UI.Owner = this;
            if (this.LoadComponent(out _action)) Action.Owner = this;
            if (this.LoadComponent(out _stats)) Stats.Owner = this;
            if (this.LoadComponent(out _statusEffect)) StatusEffect.Owner = this;
            if (this.LoadComponent(out _equipment)) Equipment.Owner = this;
            if (this.LoadComponent(out _inventory)) Inventory.Owner = this;
            if (this.LoadComponent(out _spoils)) Spoils.Owner = this;
        }

        protected override void Start()
        {
            base.Start();
            if (PhysicController != null) PhysicController.OnLand += PhysicController_OnLand;
            if (UI != null) UI.Register();
            if (Stats != null)
            {
                Stats.LoadBaseStats();
                this.InitStats();
            }
        }

        protected virtual async Task Load()
        {
            await SaveLoadManager.Instance.WaitUntilLoadedAsync();
            
        }


        public virtual void BeforeSendDamage(DamageContainer container)
        {
            OnBeforeSendDamage?.Invoke(this, container);
        }
        public virtual void AfterSendDamage(DamageContainer container)
        {
            OnAfterSendDamage?.Invoke(this, container);
        }

        public virtual void BeforeTakeDamage(DamageContainer container)
        {
            OnBeforeTakeDamage?.Invoke(this, container);
        }

        public virtual void AfterTakeDamage(DamageContainer container)
        {
            OnAfterTakeDamage?.Invoke(this, container);
            if (this.Stats.HealthGroup.Health.IsEmpty) this.Status.SetStatus(EntityStatusType.Dead);
        }

        protected virtual void InitStats()
        {
            Stats.SustenanceGroup.Hunger.OnCurrentValueChanged += Hunger_OnCurrentValueChanged;
            Stats.SustenanceGroup.Thirst.OnCurrentValueChanged += Thirst_OnCurrentValueChanged;
        }

        protected virtual void PhysicController_OnLand(object sender)
        {
            Sounds.AudioManager.Instance.PlaySFX("Creature Landing", transform.position);
            CombatSystem.DealFallingDamage(this, PhysicController.currentVelocity.y);
        }

        protected virtual void Hunger_OnCurrentValueChanged(object sender, Managers.ValueChangedEventArgs args)
        {
            TimeBasedResourceStat hunger = sender as TimeBasedResourceStat;
            if (hunger.IsEmpty) StatusEffectsManager.Instance.SendEffect<Hungry_StatusEffect>(null, this, new EffectDataContainer()
            {
                Strength = 0.1f,
                Duration = float.PositiveInfinity,
            });
            else StatusEffectsManager.Instance.RemoveEffect<Hungry_StatusEffect>(this);

            if (hunger.Ratio >= 0.8f)
            {
                if (Stats.HealthGroup.HealScale.FindAgents(null, "self full") == null)
                    Stats.HealthGroup.HealScale.AddAgent(null, "self full", 1f, StatValueType.Ratio);
            }
            else Stats.HealthGroup.HealScale.RemoveAllAgents(null, "self full");
        }

        protected virtual void Thirst_OnCurrentValueChanged(object sender, Managers.ValueChangedEventArgs args)
        {
            TimeBasedResourceStat thirst = sender as TimeBasedResourceStat;
            if (thirst.IsEmpty) StatusEffectsManager.Instance.SendEffect<Thirsty_StatusEffect>(null, this, new EffectDataContainer()
            {
                Strength = 10f,
                Duration = float.PositiveInfinity,
            });
            else StatusEffectsManager.Instance.RemoveEffect<Thirsty_StatusEffect>(this);

            if (thirst.Ratio >= 0.5f)
            {
                if (Stats.Stamina.ChangeStat.FindAgents(null, "self regen") == null)
                    Stats.Stamina.ChangeStat.AddAgent(null, "self regen", 2f, StatValueType.Scale);
            }
            else Stats.Stamina.ChangeStat.RemoveAllAgents(null, "self regen");
        }
    }
}
