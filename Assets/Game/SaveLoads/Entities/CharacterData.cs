using Asce.Game.Entities.Characters;
using Asce.Game.Items;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class CharacterData : SaveData, ISaveData<Character>, ILoadData<Character>
    {
        public Vector2 position;
        public List<ItemData> inventory = new();

        public CharacterData() { }
        public CharacterData(Character character) 
        {
            this.Save(character);
        }

        public void Save(in Character target)
        {
            if (target == null) return;
            position = target.transform.position;

            // Set Inventory
            if (target.Inventory == null) return;
            var items = target.Inventory.Inventory.Items;
            foreach (var item in items)
            {
                ItemData itemData = new(item);
                inventory.Add(itemData);
            }
        }

        public bool Load(Character character)
        {
            if (character == null) return false;
            character.transform.position = this.position;

            List<Item> items = new();
            foreach (ItemData itemData in this.inventory)
            {
                Item item = itemData.Create();
                items.Add(item);
            }
            character.Inventory.Inventory.Load(items);
            return true;
        }

    }
}