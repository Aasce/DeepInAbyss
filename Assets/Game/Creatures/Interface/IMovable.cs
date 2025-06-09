using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IMovable : IEntity
    {
        public bool IsMoveEnabled { get; set; }
        public bool IsMoving { get; }
        public bool IsIdle { get; }

        public void Moving(Vector2 direction);
    }
}
