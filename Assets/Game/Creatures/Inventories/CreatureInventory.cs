using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureInventory : GameComponent, IHasOwner<Creature>, IInventoryController
    {
        // Ref
        [SerializeField, Readonly] protected Creature _owner;

        [Space]
        [SerializeField] protected Cooldown _pickCooldown = new(0.1f);
        [SerializeField] protected bool _pickItemEnable = true;
        [SerializeField] protected float _pickRadius = 3f;
        [SerializeField] protected LayerMask _itemLayerMask;

        [Space]
        [SerializeField] protected Inventory _inventory = new();

        protected HashSet<GameObject> _notPickableItems = new();

        public Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public Inventory Inventory => _inventory;
        public bool PickItemEnable
        {
            get => _pickItemEnable;
            set => _pickItemEnable = value;
        }

        public float PickRadius
        {
            get => _pickRadius;
            set => _pickRadius = value;
        }

        protected virtual void Awake()
        {
            _inventory.SyncInventorySlots();
        }

        protected virtual void Start()
        {
            _inventory.OnItemChanged += (sender, args) => _notPickableItems.Clear();
            _inventory.OnSlotCountChanged += (sender, args) => _notPickableItems.Clear();
            if (Owner != null)
            {
                Owner.Status.OnDeath += Owner_OnDeath;
            }
        }

        protected void Update()
        {
            this.Picking();
        }

        protected virtual void Picking()
        {
            if (!PickItemEnable) return;
            if (Owner == null) return;
            if (Owner.Status.IsDead) return;

            _pickCooldown.Update(Time.deltaTime);
            if (!_pickCooldown.IsComplete) return;

            Vector2 position = (Vector2)Owner.transform.position + Vector2.up;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, PickRadius, _itemLayerMask);
            foreach (Collider2D collider in colliders)
            {
                if (_notPickableItems.Contains(collider.gameObject)) continue;
                if (!collider.TryGetComponent(out ItemObject itemObject)) continue;
                if (!itemObject.Pickable) continue;
                if (itemObject.Item.IsNull()) continue;

                Item remainItem = _inventory.AddItem(itemObject.Item);

                if (remainItem.IsNull()) itemObject.PickedBy(Owner);
                else 
                {
                    itemObject.Item.SetQuantity(remainItem.GetQuantity());
                    _notPickableItems.Add(itemObject.gameObject);
                }
            }

            _pickCooldown.Reset();
        }

        public virtual void Drop(int index, int quantity = -1)
        {
            Item dropped = _inventory.RemoveAt(index, quantity);
            if (dropped.IsNull()) return;
            
            Vector2 position = (Vector2)Owner.transform.position + Vector2.up;
            ItemObject itemObject = ItemObjectsManager.Instance.Spawn(dropped, position);
            if (itemObject == null) return;

            itemObject.Rigidbody.AddForce(Vector2.right * Owner.Status.FacingDirectionValue * 200f);
        }

        protected virtual void Owner_OnDeath(object sender)
        {
            Vector2 position = (Vector2)Owner.transform.position + Vector2.up;
            StartCoroutine(this.DropDelay(position));
        }

        protected virtual IEnumerator DropDelay(Vector2 position)
        {
            for (int i = 0; i < _inventory.Items.Count; i++) 
            {
                Item dropped = _inventory.RemoveAt(i);
                if (dropped.IsNull()) continue;

                ItemObject itemObject = ItemObjectsManager.Instance.Spawn(dropped, position);
                if (itemObject == null) continue;

                Vector2 force = Vector2.right * Random.Range(-0.5f, 0.5f) * 200f + Vector2.up * Random.Range(0f, 1f) * 200f;
                itemObject.Rigidbody.AddForce(force);
                yield return null;
            }
        }
    }
}