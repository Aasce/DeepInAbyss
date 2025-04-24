using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterColliderController : CreatureColliderController
    {
        [Header("Climbable")]
        [SerializeField] protected LayerMask _climbableLayer;
        protected int _climbableColliderCount = 0;

        [SerializeField] private CircleCollider2D _head;

        public bool IsClimbable => _climbableColliderCount > 0;
        public CircleCollider2D Head => _head;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (LayerUtils.IsInLayerMask(collision, _climbableLayer))
            {
                _climbableColliderCount++;
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);
            if (LayerUtils.IsInLayerMask(collision, _climbableLayer))
            {
                _climbableColliderCount--;
            }
        }
    }
}
