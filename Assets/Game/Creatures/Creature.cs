using Asce.Game.Combats;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, ICreature, IOptimizedComponent,
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

            if (Stats != null) Stats.LoadBaseStats();
            if (UI != null) UI.Register();
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
    }
}
