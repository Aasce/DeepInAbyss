using Asce.Game.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    public static class DroppedSpoilsSystem
    {
        public static List<ItemStack> GetDroppedItems(SO_CreatureDroppedSpoils droppedSpoils)
        {
            List<ItemStack> dropped = new();
            foreach (DroppedSpoilsContainer drop in droppedSpoils.DroppedSpoils)
            {
                if (drop.Item == null) continue;
                if (Random.value <= drop.DropChance)
                {
                    int amount = Random.Range(drop.quantityRange.x, drop.quantityRange.y + 1);
                    dropped.Add(new ItemStack(drop.Item.Name, amount));
                }
            }
            return dropped;
        }

    }
}