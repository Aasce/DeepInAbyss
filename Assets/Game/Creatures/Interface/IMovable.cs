using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IMovable : IEntity
    {
        public bool IsMovable { get; }

        public void Move(Vector2 direction);
    }
}
