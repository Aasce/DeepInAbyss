using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterPhysicController : CreaturePhysicController
    {
        [Header("Climbable")]
        [SerializeField] protected LayerMask _climbableLayer;
        protected int _climbableColliderCount = 0;

        public bool IsClimbable => _climbableColliderCount > 0;


        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            // Check if the collider belongs to a climbable layer
            if (LayerUtils.IsInLayerMask(collision, _climbableLayer))
            {
                // Increments the climbable collider count if the collider is in the climbable layer.
               _climbableColliderCount++;
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);

            // Check if the collider belongs to a climbable layer
            if (LayerUtils.IsInLayerMask(collision, _climbableLayer))
            {
                // Decrements the climbable collider count if the collider is in the climbable layer.
                _climbableColliderCount--;
            }
        }
    }
}
