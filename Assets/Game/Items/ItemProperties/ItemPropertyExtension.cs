using Asce.Game.Crafts;
using System.Collections.Generic;

namespace Asce.Game.Items
{
    /// <summary>
    ///     Extension methods for working with item properties and property data based on type.
    /// </summary>
    public static class ItemPropertyExtension
    {
        /// <summary>
        ///     Creates a runtime item property instance from the specified property type.
        /// </summary>
        /// <param name="type"> The type of property to create. </param>
        /// <returns> A new instance of the corresponding <see cref="ItemProperty"/> subclass, or null if unsupported. </returns>
        public static ItemProperty CreatePropertyFromType(this ItemPropertyType type)
        {
            return type switch
            {
                ItemPropertyType.Stackable => new StackableItemProperty(),
                ItemPropertyType.Durabilityable => new DurabilityableItemProperty(),
                ItemPropertyType.Craftable => new CraftableItemProperty(),
                ItemPropertyType.Enchantable => new EnchantableItemProperty(),
                _ => null,
            };
        }

        /// <summary>
        ///     Creates runtime property data for the specified item information and property type.
        /// </summary>
        /// <param name="information"> The item information ScriptableObject. </param>
        /// <param name="type">        The property type to extract data for. </param>
        /// <returns> A new <see cref="ItemPropertyData"/> instance, or null if not found or unsupported. </returns>
        public static ItemPropertyData CreatePropertyDataFromType(this SO_ItemInformation information, ItemPropertyType type)
        {
            if (information == null) return null;

            switch (type)
            {
                case ItemPropertyType.Stackable:
                    if (information.GetPropertyByType(ItemPropertyType.Stackable) is not StackableItemProperty stackProperty)
                        return null;
                    return new StackPropertyData(stackProperty);

                case ItemPropertyType.Durabilityable:
                    if (information.GetPropertyByType(ItemPropertyType.Durabilityable) is not DurabilityableItemProperty durabilityProperty)
                        return null;
                    return new DurabilityPropertyData(durabilityProperty);

                case ItemPropertyType.Enchantable:
                    if (information.GetPropertyByType(ItemPropertyType.Enchantable) is not EnchantableItemProperty enchantProperty)
                        return null;
                    return new EnchantPropertyData(enchantProperty);

                case ItemPropertyType.Craftable:
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Gets the maximum stack size of an item from its information.
        /// </summary>
        /// <param name="information"> The item information. </param>
        /// <returns> The maximum stack count. Returns 1 if not stackable or invalid. </returns>
        public static int GetMaxStack(this SO_ItemInformation information)
        {
            if (information == null) return 1;
            if (!information.PropertyType.HasFlag(ItemPropertyType.Stackable)) return 1;

            StackableItemProperty property = information.GetProperty<StackableItemProperty>();
            if (property == null) return 1;

            return property.MaxStack;
        }

        /// <summary>
        ///     Gets the maximum durability value of an item.
        /// </summary>
        /// <param name="information"> The item information. </param>
        /// <returns> The maximum durability. Returns 0 if not durabilityable or invalid. </returns>
        public static float GetMaxDurability(this SO_ItemInformation information)
        {
            if (information == null) return 0;
            if (!information.PropertyType.HasFlag(ItemPropertyType.Durabilityable)) return 0;

            DurabilityableItemProperty property = information.GetProperty<DurabilityableItemProperty>();
            if (property == null) return 0;

            return property.MaxDurability;
        }

        /// <summary>
        ///     Retrieves the crafting recipe ingredients from item information.
        /// </summary>
        /// <param name="information"> The item information. </param>
        /// <returns>
        ///     A new list of <see cref="Ingredient"/>s required to craft the item,
        ///     or null if not craftable or invalid.
        /// </returns>
        public static List<Ingredient> GetRecipes(this SO_ItemInformation information)
        {
            if (information == null) return null;
            if (!information.PropertyType.HasFlag(ItemPropertyType.Durabilityable)) return null;

            CraftableItemProperty property = information.GetProperty<CraftableItemProperty>();
            if (property == null) return null;

            return new List<Ingredient>(property.Ingredients);
        }
    }
}
