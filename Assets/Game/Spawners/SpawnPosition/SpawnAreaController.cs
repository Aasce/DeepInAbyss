using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class SpawnAreaController : SpawnPositionController
    {
        [SerializeField] protected Box _areaBox = new (Vector2.zero, Vector2.one * 10f);
        private Vector2 _origin;

        public virtual Box Box => _areaBox;

        public virtual Vector2 Origin
        {
            get => _origin;
            protected set => _origin = value;
        }
        public float BottomY => transform.position.y + Box.Offset.y - Box.Size.y * 0.5f;
        public float RayCastDistance => Origin.y - BottomY;

        public override Vector2 GetPosition()
        {
            Vector2 localPoint = Box.RandomPointInBox();
            Origin = (Vector2)transform.position + localPoint;

            Vector2? hitPoint = this.RaycastFrom(Origin, RayCastDistance);
            if (hitPoint.HasValue) return hitPoint.Value;

            float topY = transform.position.y + Box.Offset.y + Box.Size.y * 0.5f;
            Origin = new Vector2(Origin.x, topY);
            hitPoint = this.RaycastFrom(Origin, RayCastDistance);
            return hitPoint ?? Origin;
        }
    }
}
