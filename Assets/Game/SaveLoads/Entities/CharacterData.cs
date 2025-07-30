using Asce.Game.Entities;
using Asce.Game.Entities.Characters;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class CharacterData : SaveData, ISaveData<Character>, ILoadData<Character>
    {
        public Vector2 position;
        public EntityStatusType statusType = EntityStatusType.Alive;
        public StatsData stats = new();
        public InventoryData inventory = new();
        public EquiomentData equipment = new();
        public StatusEffectsData statusEffects = new();

        public CharacterData() { }
        public CharacterData(Character character)
        {
            this.Save(character);
        }

        public void Save(in Character target)
        {
            if (target == null) return;
            position = target.transform.position;
            statusType = target.Status.CurrentStatus;

            if (target.Stats != null) stats.Save(target.Stats);
            if (target.Inventory.Inventory != null) inventory.Save(target.Inventory.Inventory);
            if (target.Equipment != null) equipment.Save(target.Equipment);
            if (target.StatusEffect != null) statusEffects.Save(target.StatusEffect);
        }

        public bool Load(Character character)
        {
            if (character == null) return false;
            character.transform.position = this.position;
            character.Status.SetStatus(this.statusType);

            stats.Load(character.Stats);
            inventory.Load(character.Inventory.Inventory);
            equipment.Load(character.Equipment);
            statusEffects.Load(character.StatusEffect);

            character.IsLoaded = true;
            return true;
        }
    }
}