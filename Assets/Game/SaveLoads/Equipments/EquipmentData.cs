
using Asce.Game.Entities;
using Asce.Game.Equipments;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class EquiomentData: SaveData, ISaveData<IEquipmentController>, ILoadData<IEquipmentController>
    {
        public ItemData _head;
        public ItemData _chest;
        public ItemData _legs;
        public ItemData _feets;
        public ItemData _backpack;
        public ItemData _weapon;

        public void Save(in IEquipmentController equipment)
        {
            if (equipment == null) return;
            if (equipment is IHasHeadSlot headSlot) _head = new ItemData(headSlot.HeadSlot.EquipmentItem);
            if (equipment is IHasChestSlot chestSlot) _chest = new ItemData(chestSlot.ChestSlot.EquipmentItem);
            if (equipment is IHasLegsSlot legsSlot) _legs = new ItemData(legsSlot.LegsSlot.EquipmentItem);
            if (equipment is IHasFeetsSlot feetsSlot) _feets = new ItemData(feetsSlot.FeetsSlot.EquipmentItem);
            if (equipment is IHasBackpackSlot backpackSlot) _backpack = new ItemData(backpackSlot.BackpackSlot.EquipmentItem);
            if (equipment is IHasWeaponSlot weaponSlot) _weapon = new ItemData(weaponSlot.WeaponSlot.EquipmentItem);
        }

        public bool Load(IEquipmentController equipment)
        {
            if (equipment == null) return false;
            if (equipment is IHasHeadSlot headSlot && _head != null) 
                (headSlot.HeadSlot as IReceiveData<Items.Item>).Receive(_head.Create());

            if (equipment is IHasChestSlot chestSlot && _chest != null) 
                (chestSlot.ChestSlot as IReceiveData<Items.Item>).Receive(_chest.Create());

            if (equipment is IHasLegsSlot legsSlot && _legs != null) 
                (legsSlot.LegsSlot as IReceiveData<Items.Item>).Receive(_legs.Create());

            if (equipment is IHasFeetsSlot feetsSlot && _feets != null) 
                (feetsSlot.FeetsSlot as IReceiveData<Items.Item>).Receive(_feets.Create());

            if (equipment is IHasBackpackSlot backpackSlot && _backpack != null) 
                (backpackSlot.BackpackSlot as IReceiveData<Items.Item>).Receive(_backpack.Create());

            if (equipment is IHasWeaponSlot weaponSlot && _weapon != null) 
                (weaponSlot.WeaponSlot as IReceiveData<Items.Item>).Receive(_weapon.Create());

            return true;
        }
    }
}