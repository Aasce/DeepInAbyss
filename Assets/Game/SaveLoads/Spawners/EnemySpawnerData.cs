using Asce.Game.Entities;
using Asce.Game.Spawners;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class EnemySpawnerData : SaveData, ISaveData<EnemySpawner>, ILoadData<EnemySpawner>
    {
        public string id;
        public List<CreatureData> creatures = new();

        public void Save(in EnemySpawner target)
        {
            if (target == null) return;
            id = target.ID;
            foreach(Creature creature in target.Entities)
            {
                if (creature == null) continue;
                CreatureData data = new();
                data.Save(creature);
                creatures.Add(data);
            }
        }

        public bool Load(EnemySpawner target)
        {
            if (target == null) return false;
            foreach(CreatureData data in creatures)
            {
                if (data == null) continue;
                Creature creature = target.Spawn();
                data.Load(creature);
            }
            return true;
        }
    }
}