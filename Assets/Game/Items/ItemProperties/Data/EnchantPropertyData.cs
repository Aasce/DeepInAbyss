namespace Asce.Game.Items
{
    public class EnchantPropertyData : ItemPropertyData
    {


        public new EnchantableItemProperty Property => base.Property as EnchantableItemProperty;


        public EnchantPropertyData(ItemProperty property) : base(property) { }
    }
}