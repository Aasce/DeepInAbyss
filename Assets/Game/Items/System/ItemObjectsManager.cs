using Asce.Managers;
using Asce.Managers.Pools;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ItemObjectsManager : MonoBehaviourSingleton<ItemObjectsManager>
    {
        [SerializeField] protected SO_ItemsData _itemData;
        [SerializeField] protected Pool<ItemObject> _pools = new();

        [Space]
        [SerializeField] protected float _itemBlinkTime = 7f;
        [SerializeField] protected float _itemNonPickableTime = 1f;

        public SO_ItemsData ItemData => _itemData;
        public Pool<ItemObject> Pool => _pools;


        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Update()
        {
            for (int i = Pool.Activities.Count - 1; i >= 0; i--)
            {
                ItemObject itemObject = Pool.Activities[i];
                if (itemObject == null)
                {
                    Pool.Activities.RemoveAt(i);
                    continue;
                }
                if (!itemObject.AutoDespawn) continue;
                if (itemObject.IsPicked)
                {
                    this.Despawn(itemObject, i);
                    continue;
                }

                itemObject.DespawnCooldown.Update(Time.deltaTime);
                if (itemObject.DespawnCooldown.CurrentTime <= _itemBlinkTime) itemObject.View.IsBlinking(true);
                else itemObject.View.IsBlinking(false);

                if (itemObject.DespawnCooldown.BaseTime - itemObject.DespawnCooldown.CurrentTime <= _itemNonPickableTime) itemObject.Pickable = false;
                else itemObject.Pickable = true;

                if (itemObject.DespawnCooldown.IsComplete) this.Despawn(itemObject, i);
            }
        }

        public virtual ItemObject Spawn(string itemName, Vector2 position, bool isAutoDespawn = true)
        {
            SO_ItemInformation itemInfo = ItemData.GetItemByName(itemName);
            if (itemInfo == null) return null;

            Item item = new (itemInfo);
            return Spawn(item, position, isAutoDespawn);
        }

        public virtual ItemObject Spawn(Item item, Vector2 position, bool isAutoDespawn = true)
        {
            if (item.IsNull()) return null;

            ItemObject itemObject = Pool.Activate();
            if (itemObject == null) return null;

            itemObject.SetItem(item);

            itemObject.AutoDespawn = isAutoDespawn;
            itemObject.transform.position = position;
            itemObject.Refresh();

            itemObject.gameObject.SetActive(true);

            return itemObject;
        }


        public virtual void Despawn(ItemObject itemObject)
        {
            Pool.Deactivate(itemObject);
            itemObject.gameObject.SetActive(false);
        }

        protected void Despawn(ItemObject itemObject, int index)
        {
            Pool.DeactivateAt(index);
            itemObject.gameObject.SetActive(false);
        }
    }
}
