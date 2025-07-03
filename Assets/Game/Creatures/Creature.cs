using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, ICreature, IOptimizedComponent,
        IHasView<CreatureView>, IHasAction<CreatureAction>, IHasStats<CreatureStats, SO_CreatureBaseStats>, IHasEquipment<CreatureEquipment>, IHasUI<CreatureUI>
    {
        [SerializeField, Readonly] private CreaturePhysicController _physicController;
        [SerializeField, Readonly] private CreatureAction _action;
        [SerializeField, Readonly] private CreatureView _view;
        [SerializeField, Readonly] private CreatureStats _stats;
        [SerializeField, Readonly] private CreatureEquipment _equipment;
        [SerializeField, Readonly] private CreatureUI _ui;
        private bool _isControled = false;

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

        public CreatureStats Stats
        {
            get => _stats;
            set => _stats = value;
        }

        public CreatureEquipment Equipment
        {
            get => _equipment;
            set => _equipment = value;
        }

        public CreatureUI UI
        {
            get => _ui;
            set => _ui = value;
        }

        public bool IsControled
        {
            get => _isControled;
            set => _isControled = value;
        }

        protected virtual void Reset()
        {
            if (transform.LoadComponent(out _physicController))
            {
                PhysicController.Owner = this;
            }
            if (transform.LoadComponent(out _view))
            {
                View.Owner = this;
            }
            if (transform.LoadComponent(out _action))
            {
                Action.Owner = this;
            }
            if (transform.LoadComponent(out _stats))
            {
                Stats.Owner = this;
            }
            if (transform.LoadComponent(out _equipment))
            {
                Equipment.Owner = this;
            }
            if (transform.LoadComponent(out _ui))
            {
                UI.Owner = this;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();

        }
    }
}
