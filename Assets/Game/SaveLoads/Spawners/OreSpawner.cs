using Asce.Game.Entities.Ores;
using Asce.Game.Spawners;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class OreSpawnerData : SaveData, ISaveData<OreSpawner>, ILoadData<OreSpawner>
    {
        public string id;
        public List<OreData> ores = new();

        public void Save(in OreSpawner target)
        {
            if (target == null) return;
            id = target.ID;
            foreach (Ore ore in target.Entities)
            {
                if (ore == null) continue;
                OreData data = new ();
                data.Save(ore);
                ores.Add(data);
            }
        }

        public bool Load(OreSpawner target)
        {
            if (target == null) return false;
            foreach (OreData data in ores)
            {
                if (data == null) continue;
                Ore ore = target.Spawn();
                data.Load(ore);
            }
            return true;
        }
    }
}