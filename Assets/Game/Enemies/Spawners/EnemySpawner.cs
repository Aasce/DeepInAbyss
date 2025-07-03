using Asce.Game.Entities;
using Asce.Game.Entities.Enemies;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class EnemySpawner : EntitySpawner<Enemy>, IOptimizedComponent
    {
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.ActivateOutsideView;

        public override Enemy Spawn(Vector2 position)
        {
            Enemy spawnedEnemy = base.Spawn(position);
            if (spawnedEnemy == null) return null;

            spawnedEnemy.Status.SetStatus(EntityStatusType.Alive);
            spawnedEnemy.Status.SpawnPosition = spawnedEnemy.transform.position;
            spawnedEnemy.Status.OnDeath += SpawnEnemy_OnDeath;
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
            obj.Status.OnDeath -= SpawnEnemy_OnDeath;
            base.DespawnImmediately(obj);
            obj.gameObject.SetActive(false);
        }

        private void SpawnEnemy_OnDeath(object sender)
        {
            Enemy enemy = sender as Enemy;
            if (enemy == null) return;

            this.Despawn(enemy);
        }


        void IOptimizedComponent.SetActivate(bool state)
        {
            this.enabled = state;
            foreach (Enemy enemy in _pools.Activities)
            {
                if (enemy == null) continue;
                if (enemy is IOptimizedComponent optimizedComponent)
                {
                    optimizedComponent.SetActivate(!state); // because spawner is EnableOutsideView
                }
            }
        }
    }
}
