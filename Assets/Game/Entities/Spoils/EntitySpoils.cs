using Asce.Game.Items;
using Asce.Managers;
using Asce.Managers.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class EntitySpoils : GameComponent, IHasOwner<Entity>
    {
        [SerializeField, Readonly] protected Entity _owner;
        [SerializeField] protected SO_DroppedSpoils _droppedSpoils;

        [Space]
        [SerializeField] protected float _forceScale = 100f;

        public Entity Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public SO_DroppedSpoils DroppedSpoils => _droppedSpoils;


        protected virtual void Start()
        {
            if (Owner != null)
            {
                Owner.Status.OnDeath += Status_OnDeath;
            }
        }

        protected virtual void Status_OnDeath(object sender)
        {
            if (DroppedSpoils == null) return;

            List<Item> dropped = DroppedSpoilsSystem.GetDroppedItems(DroppedSpoils);
            if (dropped.Count <= 0) return;

            Vector2 spawnPosition = (Vector2)Owner.gameObject.transform.position + Vector2.up;
            StartCoroutine(this.DroppedSpoilDelay(dropped, spawnPosition, 0.02f));
        }

        protected virtual IEnumerator DroppedSpoilDelay(List<Item> dropped, Vector2 spawnPosition, float delay)
        {
            foreach (Item item in dropped)
            {
                if (item.IsNull()) continue;

                ItemObject itemObject = ItemObjectsManager.Instance.Spawn(item, spawnPosition);
                if (itemObject == null) continue;

                Vector2 randomForce = new(Random.Range(-0.5f, 0.5f), Random.Range(-0.1f, 0.5f));
                itemObject.Rigidbody.AddForce(randomForce * _forceScale);

                if (delay >= 0f) yield return new WaitForSeconds(delay);
            }
        }
    }
}