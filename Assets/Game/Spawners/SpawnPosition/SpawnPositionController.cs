using UnityEngine;

namespace Asce.Game.Spawners
{
    public abstract class SpawnPositionController : MonoBehaviour
    {
        [SerializeField] protected LayerMask _raycastLayerMask;

        public LayerMask RaycastLayerMask
        {
            get => _raycastLayerMask;
            set => _raycastLayerMask = value;
        }


        public abstract Vector2 GetPosition();

        protected virtual Vector2? RaycastFrom(Vector2 origin, float raycastDistance)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, RaycastLayerMask);

            return hit.collider != null ? hit.point : null;
        }
    }
}
