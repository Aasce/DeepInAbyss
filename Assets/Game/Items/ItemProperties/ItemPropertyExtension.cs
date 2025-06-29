namespace Asce.Game.Items
{
    public static class ItemPropertyExtension
    {
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
    }
}