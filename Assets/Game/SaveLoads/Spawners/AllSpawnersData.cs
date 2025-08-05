using Asce.Game.Spawners;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllSpawnersData : SaveData
    {
        public List<EnemySpawnerData> enemySpawners = new();
        public List<OreSpawnerData> oreSpawners = new();

        public AllSpawnersData()
        {
            List<EnemySpawner> enemySpawners = ComponentUtils.FindAllComponentsInScene<EnemySpawner>();
            foreach (EnemySpawner spawner in enemySpawners)
            {
                if (spawner == null) continue;
                EnemySpawnerData data = new();
                data.Save(spawner);
                this.enemySpawners.Add(data);
            }

            List<OreSpawner> oreSpawners = ComponentUtils.FindAllComponentsInScene<OreSpawner>();
            foreach (OreSpawner spawner in oreSpawners)
            {
                if (spawner == null) continue;
                OreSpawnerData data = new();
                data.Save(spawner);
                this.oreSpawners.Add(data);
            }
        }


        public void Load()
        {
            List<EnemySpawner> enemySpawners = ComponentUtils.FindAllComponentsInScene<EnemySpawner>();
            foreach (EnemySpawner spawner in enemySpawners)
            {
                if (spawner == null) continue;
                EnemySpawnerData matchData = this.enemySpawners.Find((s) => s.id == spawner.ID);
                if (matchData == null) continue;
                matchData.Load(spawner);
            }

            List<OreSpawner> oreSpawners = ComponentUtils.FindAllComponentsInScene<OreSpawner>();
            foreach (OreSpawner spawner in oreSpawners)
            {
                if (spawner == null) continue;
                OreSpawnerData matchData = this.oreSpawners.Find((s) => s.id == spawner.ID);
                if (matchData == null) continue;
                matchData.Load(spawner);
            }
        }
    }
}