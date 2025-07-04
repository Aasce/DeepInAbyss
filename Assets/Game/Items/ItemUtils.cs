namespace Asce.Game.Items
{
    /// <summary>
    ///     Utility extension methods for manipulating item properties such as stack quantity and durability.
    /// </summary>
    public static class ItemUtils
    {
        /// <summary>
        ///     Gets the quantity from a stackable item.
        /// </summary>
        /// <param name="item"> The item to query. </param>
        /// <returns> The quantity of the stack. Returns 0 if the item is null or not stackable. </returns>
        public static int GetQuantity(this Item item)
        {
            if (item == null) return 0;

            // Retrieve the stack property data
            StackPropertyData stackData = item.GetProperty<StackPropertyData>(ItemPropertyType.Stackable);
            if (stackData == null) return 0;

            return stackData.Quantity;
        }

        /// <summary>
        ///     Sets the quantity for a stackable item.
        /// </summary>
        /// <param name="item">     The item to update. </param>
        /// <param name="quantity"> The new quantity to set. </param>
        public static void SetQuantity(this Item item, int quantity)
        {
            if (item == null) return;

            // Retrieve the stack property and assign new quantity
            StackPropertyData stackData = item.GetProperty<StackPropertyData>(ItemPropertyType.Stackable);
            if (stackData == null) return;

            stackData.Quantity = quantity;
        }

        /// <summary>
        ///     Gets the durability value of an item.
        /// </summary>
        /// <param name="item"> The item to query. </param>
        /// <returns> The durability value. Returns 0 if the item is null or has no durability property. </returns>
        public static float GetDurability(this Item item)
        {
            if (item == null) return 0;

            // Retrieve the durability property data
            DurabilityPropertyData durabilityData = item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Stackable);
            if (durabilityData == null) return 0;

            return durabilityData.Durability;
        }

        /// <summary>
        ///     Sets the durability value of an item.
        /// </summary>
        /// <param name="item">       The item to update. </param>
        /// <param name="durability"> The new durability value. </param>
        public static void SetDurability(this Item item, float durability)
        {
            if (item == null) return;

            // Retrieve the durability property and assign new value
            DurabilityPropertyData durabilityData = item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Stackable);
            if (durabilityData == null) return;

            durabilityData.Durability = durability;
        }
    }
}
