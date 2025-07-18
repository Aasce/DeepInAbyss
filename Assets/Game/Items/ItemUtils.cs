namespace Asce.Game.Items
{
    /// <summary>
    ///     Utility extension methods for manipulating item properties such as stack quantity and durability.
    /// </summary>
    public static class ItemUtils
    {
        /// <summary>
        ///     Check is <see cref="Item"/> is null or not.
        /// </summary>
        /// <param name="item"> Item to be check. </param>
        /// <returns>
        ///     Returns true if <paramref name="item"/> is null and <see cref="Item.Information"/> is null.
        /// </returns>
        public static bool IsNull(this Item item)
        {
            if (item == null) return true;
            if (item.Information == null) return true;
            return false;
        }

        public static Item Clone(this Item item)
        {
            if (item.IsNull()) return null;

            Item newItem = new(item.Information);
            newItem.SetQuantity(item.GetQuantity());
            newItem.SetDurability(item.GetDurability());
            return newItem;
        }

        public static (Item firstItem, Item secondItem) Split(this Item item, int quantity)
        {
            if (item.IsNull()) return (null, null);

            Item copyItem = item.Clone();

            if (!item.HasQuantity()) return (copyItem, null);
            if (quantity <= 0) return (copyItem, null);

            int currentQuantity = item.GetQuantity();
            if (quantity >= currentQuantity) return (null, copyItem);

            Item splitItem = new (item.Information);

            int remainQuantity = currentQuantity - quantity;
            copyItem.SetQuantity(remainQuantity);
            splitItem.SetQuantity(quantity);

            return (copyItem, splitItem);
        }

        /// <summary>
        ///     Determines whether the given item has stack data.
        /// </summary>
        /// <param name="item">
        ///     The item to check. If the item is null or does not contain stack data,
        ///     the result will be false.
        /// </param>
        /// <returns>
        ///     Returns true if the item has stack. Otherwise, returns false.
        /// </returns>
        public static bool HasQuantity(this Item item)
        {
            if (item.IsNull()) return false;

            StackPropertyData stackData = item.GetProperty<StackPropertyData>(ItemPropertyType.Stackable);
            if (stackData == null) return false;
            return true;
        }

        /// <summary>
        ///     Gets the quantity from a stackable item.
        /// </summary>
        /// <param name="item"> The item to query. </param>
        /// <returns> The quantity of the stack. Returns 0 if the item is null or not stackable. </returns>
        public static int GetQuantity(this Item item)
        {
            if (item.IsNull()) return 0;

            // Retrieve the stack property data
            StackPropertyData stackData = item.GetProperty<StackPropertyData>(ItemPropertyType.Stackable);
            if (stackData == null) return 1;

            return stackData.Quantity;
        }

        /// <summary>
        ///     Sets the quantity for a stackable item.
        /// </summary>
        /// <param name="item">     The item to update. </param>
        /// <param name="quantity"> The new quantity to set. </param>
        public static void SetQuantity(this Item item, int quantity)
        {
            if (item.IsNull()) return;

            // Retrieve the stack property and assign new quantity
            StackPropertyData stackData = item.GetProperty<StackPropertyData>(ItemPropertyType.Stackable);
            if (stackData == null) return;

            stackData.Quantity = quantity;
        }

        /// <summary>
        ///     Determines whether the given item has durability data.
        /// </summary>
        /// <param name="item">
        ///     The item to check. If the item is null or does not contain durability data,
        ///     the result will be false.
        /// </param>
        /// <returns>
        ///     Returns true if the item has durability. Otherwise, returns false.
        /// </returns>
        public static bool HasDurability(this Item item)
        {
            if (item.IsNull()) return false;

            DurabilityPropertyData durabilityData = item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durabilityData == null) return false;
            return true;
        }

        /// <summary>
        ///     Gets the durability value of an item.
        /// </summary>
        /// <param name="item"> The item to query. </param>
        /// <returns> The durability value. Returns 0 if the item is null or has no durability property. </returns>
        public static float GetDurability(this Item item)
        {
            if (item.IsNull()) return 0f;

            // Retrieve the durability property data
            DurabilityPropertyData durabilityData = item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durabilityData == null) return 0f;

            return durabilityData.Durability;
        }

        /// <summary>
        ///     Sets the durability value of an item.
        /// </summary>
        /// <param name="item"> The item to update. </param>
        /// <param name="durability"> The new durability value. </param>
        public static void SetDurability(this Item item, float durability)
        {
            if (item.IsNull()) return;

            // Retrieve the durability property and assign new value
            DurabilityPropertyData durabilityData = item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durabilityData == null) return;

            durabilityData.Durability = durability;
        }
    }
}
