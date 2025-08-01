using Asce.Game.Items;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class ItemObjectData : SaveData, ISaveData<ItemObject>, ICreateData<ItemObject>
    {
        public ItemData item;
        public Vector2 position;

        public bool isPickable;
        public bool autoDespawn;
        public float despawnDuration;

        public void Save(in ItemObject target)
        {
            if (target == null) return;
            if (target.Item.IsNull()) return;
            item = new();
            item.Save(target.Item);
            position = target.transform.position;

            isPickable = target.Pickable;
            autoDespawn = target.AutoDespawn;
            if (autoDespawn) despawnDuration = target.DespawnCooldown.CurrentTime;
        }

        public ItemObject Create()
        {
            Item item = this.item.Create();
            if (item.IsNull()) return null;

            ItemObject itemObject = ItemObjectsManager.Instance.Spawn(item, position, autoDespawn);
            if (itemObject == null) return null;
            if (autoDespawn) itemObject.DespawnCooldown.CurrentTime = despawnDuration;
            itemObject.Pickable = isPickable;

            return itemObject;
        }
    }
}