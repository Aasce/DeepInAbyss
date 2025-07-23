using Asce.Game.Entities;

namespace Asce.Game.Equipments
{
    public static class EquipmentExtension
    {
        public static EquipmentSlot GetSlot(this IEquipmentController controller, EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    if (controller is IHasWeaponSlot weapon) return weapon.WeaponSlot;
                    break;
                case EquipmentType.Helmet:
                    if (controller is IHasHeadSlot head) return head.HeadSlot;
                    break;
                case EquipmentType.Chest:
                    if (controller is IHasChestSlot chest) return chest.ChestSlot;
                    break;
                case EquipmentType.Legging:
                    if (controller is IHasLegsSlot legs) return legs.LegsSlot;
                    break;
                case EquipmentType.Boots:
                    if (controller is IHasFeetsSlot feets) return feets.FeetsSlot;
                    break;
                case EquipmentType.Backpack:
                    if (controller is IHasBackpackSlot backpack) return backpack.BackpackSlot;
                    break;
            }

            return null;
        }

        public static T GetSlot<T>(this IEquipmentController controller, EquipmentType type) where T : EquipmentSlot
        {
            var slot = controller.GetSlot(type);
            if (slot is T typedSlot)
            {
                return typedSlot;
            }
            return null;
        }
    }
}