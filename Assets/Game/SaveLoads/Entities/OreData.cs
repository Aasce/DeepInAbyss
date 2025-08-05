using Asce.Game.Entities;
using Asce.Game.Entities.Ores;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class OreData : SaveData, ISaveData<Ore>, ILoadData<Ore>
    {
        public Vector2 position;
        public EntityStatusType statusType = EntityStatusType.Alive;
        public StatsData stats = new();
        public float regenTime;

        public OreData() { }
        public OreData(Ore ore)
        {
            this.Save(ore);
        }

        public void Save(in Ore target)
        {
            if (target == null) return;
            position = target.transform.position;
            statusType = target.Status.CurrentStatus;
            regenTime = target.RegenCooldown.CurrentTime;

            if (target.Stats != null) stats.Save(target.Stats);
        }

        public bool Load(Ore ore)
        {
            if (ore == null) return false;
            ore.transform.position = this.position;
            ore.Status.SetStatus(this.statusType);
            ore.RegenCooldown.CurrentTime = regenTime;

            stats.Load(ore.Stats);

            ore.IsLoaded = true;
            return true;
        }
    }
}