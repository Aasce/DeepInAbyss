using Asce.Game.Combats;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Enemies 
{ 
    public class Enemy : Creature, IHasView<EnemyView>, IHasAction<EnemyAction>, IHasStats<EnemyStats, SO_EnemyBaseStats>, IHasUI<EnemyUI>, IHasSpoils<EnemySpoils>
    {
        [Space]
        [SerializeField] protected HitBox _damageHitBox = new();
        protected HashSet<GameObject> _damagedObject = new();

        public new EnemyPhysicController PhysicController => base.PhysicController as EnemyPhysicController;
        public new EnemyView View => base.View as EnemyView;
        public new EnemyAction Action => base.Action as EnemyAction;
        public new EnemyStats Stats => base.Stats as EnemyStats;
        public new EnemyUI UI => base.UI as EnemyUI;
        public new EnemySpoils Spoils => base.Spoils as EnemySpoils;


        public HitBox DamageHitBox => _damageHitBox;


        protected override void Reset()
        {
            base.Reset();
        }

        protected override void Start()
        {
            base.Start();
            Action.OnAttack += Action_OnAttack;
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void Action_OnAttack(object sender)
        {
            _damagedObject.Clear();

            Collider2D[] colliders = _damageHitBox.Hit(transform.position, Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform == transform) continue; // Ignore self
                if (_damagedObject.Contains(collider.gameObject)) continue; // Avoid dealing damage to the same creature multiple times
                if (!collider.TryGetComponent(out ICreature creature)) continue;

                CombatSystem.DamageDealing(new DamageContainer(this, creature)
                {
                    Damage = Stats.Strength.Value,
                    DamageType = DamageType.Physical,
                });

                _damagedObject.Add(collider.gameObject);
            }
        }
    }
}