using Asce.Game.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    public static class DroppedSpoilsSystem
    {
        public static List<Item> GetDroppedItems(SO_CreatureDroppedSpoils droppedSpoils)
        {
            List<Item> dropped = new();
            foreach (DroppedSpoilsContainer drop in droppedSpoils.DroppedSpoils)
            {
                if (drop.ItemInformation == null) continue;
                if (Random.value <= drop.DropChance)
                {
                    Item addingItem = new (drop.ItemInformation);

                    // Stack
                    if (drop.ItemInformation.HasProperty(ItemPropertyType.Stackable))
                    {
                        int amount = Random.Range(drop.QuantityRange.x, drop.QuantityRange.y + 1);
                        addingItem.SetQuantity(amount);
                    }
                    else addingItem.SetQuantity(1);

                    // Durability
                    if (drop.ItemInformation.HasProperty(ItemPropertyType.Durabilityable))
                    {
                        float durationRatio = Random.Range(drop.DurationRatioRange.x, drop.DurationRatioRange.y);
                        float maxDurability = addingItem.Information.GetMaxDurability();
                        addingItem.SetDurability(durationRatio * maxDurability);
                    }

                    dropped.Add(addingItem);
                }
            }
            return dropped;
        }

    }
}