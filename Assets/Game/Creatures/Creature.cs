using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, IHasView<CreatureView>, IHasAction<CreatureAction>, IHasStats<CreatureStats, SO_CreatureBaseStats>
    {
        [SerializeField, HideInInspector] private CreaturePhysicController _physicController;
        [SerializeField, HideInInspector] private CreatureView _view;
        [SerializeField, HideInInspector] private CreatureAction _action;
        [SerializeField, HideInInspector] private CreatureStats _stats;


        public CreaturePhysicController PhysicController
        {
            get => _physicController;
            set => _physicController = value;
        }

        public CreatureView View
        {
            get => _view;
            set => _view = value;
        }

        public CreatureAction Action
        {
            get => _action; 
            set => _action = value;
        }

        public CreatureStats Stats
        {
            get => _stats;
            set => _stats = value;
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
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Stats != null) Stats.ResetStats();
        }

    }
}
