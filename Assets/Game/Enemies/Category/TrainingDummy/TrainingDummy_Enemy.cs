using Asce.Game.Combats;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class TrainingDummy_Enemy : Enemy
    {
        [SerializeField] private Cooldown _refreshCooldown = new(5f);


        protected override void Start()
        {
            base.Start();
            this.OnAfterTakeDamage += Enemy_OnAfterTakeDamage;
        }

        private void Enemy_OnAfterTakeDamage(object sender, DamageContainer container)
        {
            _refreshCooldown.Reset();
        }

        protected override void Update()
        {
            base.Update();

            if (Stats.HealthGroup.Health.IsFull) return;
            _refreshCooldown.Update(Time.deltaTime);
            if (_refreshCooldown.IsComplete)
            {
                CombatSystem.Healing(this, Stats, Stats.HealthGroup.Health.Value, position: transform.position);
                Status.SetStatus(EntityStatusType.Alive);

                _refreshCooldown.Reset();
            }
        }
    }
}
