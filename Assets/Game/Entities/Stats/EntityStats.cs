using Asce.Game.Stats;
using Asce.Managers;
using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class EntityStats : GameComponent, IHasOwner<Entity>, IStatsController<SO_EntityBaseStats>
    {
        public static readonly string baseStatsReason = "base stats";

        [SerializeField, Readonly] protected Entity _owner;
        [SerializeField] protected SO_EntityBaseStats _baseStats;

        [Space]
        [SerializeField] protected bool _isStatsUpdating = true;

        public virtual Entity Owner
        {
            get => _owner;
            set => _owner = value;
        }
        public virtual SO_EntityBaseStats BaseStats => _baseStats;
        
        public virtual bool IsDead => Owner.Status.IsDead;
        public virtual bool IsStatsUpdating
        {
            get => _isStatsUpdating;
            set => _isStatsUpdating = value;
        }


        protected virtual void Start()
        {
            Owner.Status.OnDeath += Owner_OnDeath;
            Owner.Status.OnRevive += Owner_OnRevive;
        }

        protected virtual void Update()
        {
            this.UpdateStats(Time.deltaTime);
        }

        public abstract void LoadBaseStats();
        public abstract void UpdateStats(float deltaTime);
        public abstract void ClearStats(bool isForceClear = false);
        public abstract void ResetStats();

        protected virtual void Owner_OnDeath(object sender)
        {
            IsStatsUpdating = false;
        }

        protected virtual void Owner_OnRevive(object sender)
        {
            IsStatsUpdating = true;
            this.ResetStats();
        }
    }
}