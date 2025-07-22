using Asce.Game.Combats;
using Asce.Game.Enviroments;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities.Ores
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Ore : Entity, IOre, IEnviromentComponent,
        IHasUI<OreUI>, IHasSpoils<OreSpoils>,
        IHasStats<OreStats, SO_OreBaseStats>, ITakeDamageable
    {
        [SerializeField, Readonly] protected BoxCollider2D _collider;
        [SerializeField, Readonly] protected OreStats _stats;
        [SerializeField, Readonly] protected OreSpoils _spoils;
        [SerializeField, Readonly] protected OreUI _ui;

        [Space]
        [SerializeField] protected VFXs.VFXObject _deathVFX;

        [Header("Regeneration")]
        [SerializeField] protected Cooldown _regenCooldown = new(30f);

        public event Action<object, DamageContainer> OnBeforeTakeDamage;
        public event Action<object, DamageContainer> OnAfterTakeDamage;


        public BoxCollider2D Collider => _collider;
        public OreStats Stats => _stats;
        public OreSpoils Spoils => _spoils;
        public OreUI UI => _ui;

        public bool IsDead => Status.IsDead;

        public Cooldown RegenCooldown => _regenCooldown;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _collider);
            if (this.LoadComponent(out _stats)) Stats.Owner = this;
            if (this.LoadComponent(out _spoils)) Spoils.Owner = this;
            if (this.LoadComponent(out _ui)) UI.Owner = this;
        }
        protected override void Start()
        {
            base.Start();

            if (Stats != null) Stats.LoadBaseStats();
            if (UI != null) UI.Register();
        }

        private void Update()
        {
            if (Stats.HealthGroup.Health.IsFull) return;
            RegenCooldown.Update(Time.deltaTime);
            if (RegenCooldown.IsComplete)
            {
                CombatSystem.Healing(this, Stats, transform.position, Stats.HealthGroup.Health.Value);
                Status.SetStatus(EntityStatusType.Alive);
                RegenCooldown.Reset();
            }
        }

        public void BeforeTakeDamage(DamageContainer container)
        {
            OnBeforeTakeDamage?.Invoke(this, container);
        }
        public void AfterTakeDamage(DamageContainer container)
        {
            RegenCooldown.Reset();
            OnAfterTakeDamage?.Invoke(this, container);
            if (this.Stats.HealthGroup.Health.IsEmpty)
            {
                if (_deathVFX != null) VFXs.VFXsManager.Instance.Spawn(_deathVFX, transform.position, Quaternion.identity);
                this.Status.SetStatus(EntityStatusType.Dead);
            }
        }
    }
}
