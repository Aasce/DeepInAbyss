namespace Asce.Game.Items
{
    [System.Flags]
    public enum ItemPropertyType 
    {
        None = 0,

        Stackable = 1 << 1,

        QuestLocked = 1 << 3,
        Tradable = 1 << 4,

        Usable = 1 << 7,
        Equippable = 1 << 8,

        Craftable = 1 << 10,
        Enchantable = 1 << 11,
        Upgradeable = 1 << 12,
        Durabilityable = 1 << 13,
    }
} 