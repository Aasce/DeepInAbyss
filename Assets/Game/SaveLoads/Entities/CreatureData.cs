using Asce.Game.Entities;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class CreatureData : SaveData, ISaveData<Creature>, ILoadData<Creature>
    {
        public Vector2 position;
        public EntityStatusType statusType = EntityStatusType.Alive;
        public StatsData stats = new();
        public InventoryData inventory = new();
        public EquiomentData equipment = new();
        public StatusEffectsData statusEffects = new();

        public CreatureData() { }
        public CreatureData(Creature creature)
        {
            this.Save(creature);
        }

        public void Save(in Creature target)
        {
            if (target == null) return;
            position = target.transform.position;
            statusType = target.Status.CurrentStatus;

            if (target.Stats != null) stats.Save(target.Stats);
            if (target.Inventory.Inventory != null) inventory.Save(target.Inventory.Inventory);
            if (target.Equipment != null) equipment.Save(target.Equipment);
            if (target.StatusEffect != null) statusEffects.Save(target.StatusEffect);
        }

        public bool Load(Creature creature)
        {
            if (creature == null) return false;
            creature.transform.position = this.position;
            creature.Status.SetStatus(this.statusType);

            stats.Load(creature.Stats);
            inventory.Load(creature.Inventory.Inventory);
            equipment.Load(creature.Equipment);
            statusEffects.Load(creature.StatusEffect);

            return true;
        }
    }
}