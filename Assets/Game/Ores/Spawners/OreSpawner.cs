using Asce.Game.Entities.Ores;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class OreSpawner : EntitySpawner<Ore>
    {
        public override Ore Spawn(Vector2 position)
        {
            Ore spawnedOre = base.Spawn(position);
            if (spawnedOre == null) return null;

            spawnedOre.Status.OnDeath += Ore_OnDeath;
            spawnedOre.gameObject.SetActive(true);
            if (spawnedOre is IOptimizedComponent optimizedEntity) optimizedEntity.SetActivate(false);

            return spawnedOre;
        }

        protected override IEnumerator DespawnAfterDelay(Ore obj, float despawnDelay)
        {
            yield return new WaitForSeconds(despawnDelay);
            if (obj == null) yield break;
            if (obj.Status.IsAlive) yield break;

            DespawnImmediately(obj);
        }

        public override void DespawnImmediately(Ore obj)
        {
            obj.Status.OnDeath -= Ore_OnDeath;
            base.DespawnImmediately(obj);
            obj.gameObject.SetActive(false);
        }

        private void Ore_OnDeath(object sender)
        {
            Ore ore = sender as Ore;
            if (ore == null) return;

            this.Despawn(ore);
        }
    }
}
