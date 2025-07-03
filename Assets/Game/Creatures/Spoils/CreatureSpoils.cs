using Asce.Game.Items;
using Asce.Managers.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureSpoils : MonoBehaviour, IHasOwner<Creature>
    {
        [SerializeField, Readonly] protected Creature _owner;
        [SerializeField] protected SO_CreatureDroppedSpoils _droppedSpoils;

        [Space]
        [SerializeField] protected float _forceScale = 100f;

        public Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public SO_CreatureDroppedSpoils DroppedSpoils => _droppedSpoils;


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

            List<ItemStack> dropped = DroppedSpoilsSystem.GetDroppedItems(DroppedSpoils);
            if (dropped.Count <= 0) return;

            Vector2 spawnPosition = (Vector2)Owner.transform.position + Vector2.up;
            StartCoroutine(this.DroppedSpoilDelay(dropped, spawnPosition, 0.02f));
        }

        protected virtual IEnumerator DroppedSpoilDelay(List<ItemStack> dropped, Vector2 spawnPosition, float delay)
        {
            foreach (ItemStack itemStack in dropped)
            {
                ItemObject item = ItemObjectsManager.Instance.Spawn(itemStack.Name, itemStack.Quantity, spawnPosition);
                if (item == null) continue;

                Vector2 randomForce = new (Random.Range(-0.5f, 0.5f), Random.Range(-0.1f, 0.5f));
                item.Rigidbody.AddForce(randomForce * _forceScale);

                if (delay >= 0f) yield return new WaitForSeconds(delay);
            }
        }
    }
}