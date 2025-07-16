using Asce.Game.Entities;
using Asce.Game.Entities.Enemies;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class EnemySpawner : CreatureSpawner<Enemy>
    {
        public override Enemy Spawn(Vector2 position)
        {
            Enemy spawnedEnemy = base.Spawn(position);
            if (spawnedEnemy == null) return null;

            spawnedEnemy.Status.SetStatus(EntityStatusType.Alive);
            spawnedEnemy.Status.SpawnPosition = spawnedEnemy.transform.position;
            spawnedEnemy.Status.OnDeath += Enemy_OnDeath;
            spawnedEnemy.gameObject.SetActive(true);
            if (spawnedEnemy is IOptimizedComponent optimizedEnemy) optimizedEnemy.SetActivate(false); 

            return spawnedEnemy;
        }

        protected override IEnumerator DespawnAfterDelay(Enemy obj, float despawnDelay)
        {
            yield return new WaitForSeconds(despawnDelay);
            if (obj == null) yield break;
            if (obj.Status.IsAlive) yield break;

            DespawnImmediately(obj);
        }

        public override void DespawnImmediately(Enemy obj)
        {
            obj.Status.OnDeath -= Enemy_OnDeath;
            base.DespawnImmediately(obj);
            obj.gameObject.SetActive(false);
        }

        private void Enemy_OnDeath(object sender)
        {
            Enemy enemy = sender as Enemy;
            if (enemy == null) return;

            this.Despawn(enemy);
        }
    }
}
